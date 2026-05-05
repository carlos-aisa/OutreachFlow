# OutreachFlow Architecture

## Architecture Style

OutreachFlow uses layered architecture with strict dependency direction.

## Layers and Responsibilities

### Domain

- Business entities, value objects, enums, and invariants.
- No framework dependencies.

### Application

- Use cases, DTOs, interfaces (ports), and orchestration logic.
- Depends only on Domain.

### Infrastructure

- EF Core DbContext, migrations, persistence adapters, and external providers.
- Implements interfaces defined in Application.

### Presentation (API)

- HTTP endpoints, request/response mapping, OpenAPI/Swagger exposure.
- Depends on Application and Infrastructure composition root.

### Presentation (Web)

- Blazor UI components and user interaction flows.
- Uses application contracts without domain rule duplication.

## Dependency Rules

- `OutreachFlow.Api -> OutreachFlow.Application`
- `OutreachFlow.Api -> OutreachFlow.Infrastructure`
- `OutreachFlow.Web -> OutreachFlow.Application`
- `OutreachFlow.Infrastructure -> OutreachFlow.Application`
- `OutreachFlow.Infrastructure -> OutreachFlow.Domain`
- `OutreachFlow.Application -> OutreachFlow.Domain`
- `OutreachFlow.Domain -> (none)`

## Persistence Strategy (MVP)

- EF Core with SQLite.
- Migrations are generated and versioned in Infrastructure.
- Production database provider abstraction remains open for PostgreSQL in later phases.

## Core Contact Model

Phase 1 introduces the reusable CRM foundation:

- `Organization` represents a generic company, association, institution, client, or prospect.
- `Contact` represents a person or email address, optionally associated with an organization.
- `Tag` represents a flexible, user-defined classification label.
- `ContactTag` stores contact-to-tag assignments.

The Domain layer owns validation and invariants such as required names, valid email addresses, contact status transitions, do-not-contact behavior, and idempotent tag assignment.

The Application layer exposes DTOs and services for organization, contact, and tag CRUD, contact filtering, duplicate email prevention, and contact tag assignment/removal.

The Infrastructure layer persists the model with EF Core. SQLite migrations include unique indexes for normalized contact email and normalized tag category/name, plus relational foreign keys for organization and tag assignments.

The API layer exposes REST endpoints under `/api/v1`. The OpenAPI source of truth is `docs/api/openapi.v1.yaml`.

The Web layer uses typed API clients and Blazor pages for contacts, organizations, and tags. Components keep local UI state and delegate API access to services.

## Email Provider Strategy (Future Phase)

- Application defines ports for outbound email sending.
- Infrastructure provides concrete providers (Fake, SMTP, Gmail API, Graph) over time.
- No provider-specific behavior in Domain or Application core logic.
