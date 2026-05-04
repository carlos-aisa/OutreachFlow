# OutreachFlow Architecture (Phase 0 Baseline)

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

## Email Provider Strategy (Future Phase)

- Application defines ports for outbound email sending.
- Infrastructure provides concrete providers (Fake, SMTP, Gmail API, Graph) over time.
- No provider-specific behavior in Domain or Application core logic.
