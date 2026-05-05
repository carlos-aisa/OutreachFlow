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

## Configurable Sender And Template Model

Phase 2 adds the configurable content layer used by later draft and sending workflows:

- `SenderProfile` stores sender identity fields such as name, email, phone, organization name, website, and signature.
- `EmailTemplate` stores reusable subject/body templates as data, not hardcoded copy.
- `TemplateVariableService` exposes the centralized list of supported template variables for API and UI guidance.

Only one active sender profile is treated as default at a time. The Application layer enforces this by clearing existing defaults before marking a new default profile.

Sender profiles and email templates are deactivated instead of hard-deleted through the API. This keeps future draft and communication history references stable.

The Web layer uses typed API clients for sender profiles and templates, and now consumes the centralized template variable catalog exposed by the API.

## Template Rendering Engine

Phase 3 introduces deterministic template rendering for personalized email generation:

- `TemplateVariableRegistry` is the single source of truth for supported variable names and value resolvers.
- `ITemplateRenderer` defines rendering as an Application-layer contract.
- `TemplateRenderer` resolves `{{variable.path}}` tokens in subject and body using `TemplateContext`.
- Rendering diagnostics are returned through `RenderedEmail`:
  - `MissingVariables`: supported variables with null/whitespace values in context.
  - `UnknownVariables`: tokens not present in the registry.
  - `HasErrors`: true when unknown, missing, or unresolved tokens remain.

The renderer intentionally supports substitution only. Expression-like tokens are treated as unknown variables and are never executed.

## Attachment Assets

Phase 4 adds reusable attachment management and template defaults:

- `AttachmentAsset` stores attachment metadata and lifecycle (`IsActive`) in Domain.
- `EmailTemplateAttachment` models many-to-many default attachment assignments for templates.
- `IAttachmentFileStorage` is the Application port for binary storage operations.
- `LocalAttachmentFileStorage` is the Infrastructure adapter for MVP local disk storage.
- `AttachmentStorage:RootPath` config controls the safe storage root used by the local adapter.

Files are stored outside the database. EF Core persists metadata and template associations in:

- `AttachmentAssets`
- `EmailTemplateAttachments`

Template assignment rules are enforced through the Domain model:

- inactive attachments cannot be assigned as defaults;
- duplicate default assignments are ignored idempotently.

## Email Draft Generation

Phase 5 introduces controlled draft generation before any send action:

- `EmailDraft` stores rendered subject/body snapshots and render diagnostics for review workflows.
- `EmailDraftAttachment` persists selected and default attachment references per generated draft.
- `EmailDraftService` orchestrates:
  - contact selection using existing filter and tag criteria;
  - template rendering via `ITemplateRenderer`;
  - skip reporting for ineligible contacts;
  - attachment validation and assignment.

Generation outputs explicit diagnostics:

- `MissingVariables`
- `UnknownVariables`
- `HasRenderErrors` and `NeedsReview` status when render issues exist.

API and Web now include draft generation endpoints and a Blazor wizard flow that mirrors the controlled process:

1. filter recipients,
2. select template and sender,
3. select optional attachments,
4. preview,
5. generate and inspect results.

## Draft Review And Approval

Phase 6 introduces the explicit human review gate before any future sending workflow:

- `EmailDraft` now supports review transitions:
  - manual content update (`UpdateContent`);
  - approval (`Approve`);
  - cancellation (`Cancel`).
- Domain approval invariants block approval when:
  - render errors remain;
  - unresolved diagnostics remain;
  - unresolved `{{...}}` tokens remain in subject or body.
- `ApprovedAt` and `CancelledAt` timestamps are persisted for traceability.

Application adds explicit review use cases (`UpdateAsync`, `ApproveAsync`, `CancelAsync`) that wrap domain rule violations as validation errors for API/UI feedback.

API now exposes draft review endpoints:

- `GET /api/v1/drafts`
- `GET /api/v1/drafts/{id}`
- `PUT /api/v1/drafts/{id}`
- `POST /api/v1/drafts/{id}/approve`
- `POST /api/v1/drafts/{id}/cancel`

Web adds dedicated review screens:

- Draft list page with status filtering.
- Draft detail page with subject/body editing and approval/cancellation actions.

## Email Provider Strategy (Future Phase)

- Application defines ports for outbound email sending.
- Infrastructure provides concrete providers (Fake, SMTP, Gmail API, Graph) over time.
- No provider-specific behavior in Domain or Application core logic.
