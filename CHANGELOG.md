# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog and this project follows Semantic Versioning.

## [0.7.0] - 2026-05-05

### Added

- Draft review domain behavior for manual edits, approval, and cancellation transitions.
- Approval guard rules that block approval when render errors, unresolved diagnostics, or unresolved `{{...}}` tokens remain.
- Draft review metadata persistence (`ApprovedAt`, `CancelledAt`) with EF migration `DraftReviewApprovalMetadata`.
- Draft review API endpoints:
  - `PUT /api/v1/drafts/{id}`
  - `POST /api/v1/drafts/{id}/approve`
  - `POST /api/v1/drafts/{id}/cancel`
- Blazor draft review UI:
  - draft list page with status filter,
  - draft detail page for subject/body editing,
  - approve/cancel actions with validation feedback.
- Domain, application, persistence, and API integration tests for draft review and approval flows.
- OpenSpec archive for `p06-draft-review-and-approval`.

### Changed

- Draft DTO/API contract now includes `approvedAt` and `cancelledAt` timestamps.
- README, architecture, and OpenAPI documentation now describe the Phase 6 human review workflow.

## [0.6.0] - 2026-05-05

### Added

- Email draft domain model (`EmailDraft`, `EmailDraftStatus`, `EmailDraftAttachment`) with generated status behavior and attachment assignment rules.
- Draft persistence with EF Core mappings and migration for `EmailDrafts` and `EmailDraftAttachments`.
- Draft generation application flow with:
  - multi-contact selection via existing contact filters and tags,
  - template rendering diagnostics persistence,
  - skipped contact reporting for ineligible recipients,
  - attachment validation and default/optional attachment assignment.
- REST endpoints for draft generation, listing, and detail:
  - `POST /api/v1/drafts/generate`
  - `GET /api/v1/drafts`
  - `GET /api/v1/drafts/{id}`
- Blazor draft generation wizard with recipient filtering, template/sender/attachment selection, preview, and result summary.
- Domain, application, and integration tests for draft creation, generation diagnostics, persistence, and API behavior.
- OpenSpec archive for `p05-email-draft-generation`.

### Changed

- OpenAPI specification now includes draft generation contracts and `EmailDraft` schemas.
- README and architecture documentation now describe Phase 5 draft generation workflow.

## [0.5.0] - 2026-05-05

### Added

- Attachment asset domain model with reusable metadata and active/inactive lifecycle behavior.
- `EmailTemplateAttachment` relationship model and domain rules for default attachment assignment.
- Application attachment contracts and services (`IAttachmentAssetService`, `IAttachmentFileStorage`, upload/update/list/deactivate use cases).
- Local disk storage adapter with safe root enforcement and path traversal rejection.
- EF Core tables, mappings, and migration for `AttachmentAssets` and `EmailTemplateAttachments`.
- REST endpoints for attachment upload/list/read/update/deactivate and template default attachment assignment/removal.
- Blazor attachment management page and template default attachment controls.
- Domain, application, and integration tests for attachment persistence, upload behavior, unsafe path rejection, and template association.
- OpenSpec archive for `p04-attachment-assets`.

### Changed

- Email template DTOs and API responses now include `defaultAttachmentIds`.
- Template repository queries now include default attachment associations.
- README, architecture, and OpenAPI documentation now include attachment storage configuration and API contracts.

## [0.4.0] - 2026-05-05

### Added

- Template rendering contracts: `ITemplateRenderer`, `TemplateContext`, and `RenderedEmail`.
- Centralized `TemplateVariableRegistry` with explicit supported variables and value resolvers.
- `TemplateRenderer` implementation for subject/body token substitution using `{{variable.path}}` syntax.
- Rendering diagnostics for unknown variables, missing values, and unresolved tokens.
- Application tests covering successful rendering, unknown variables, missing values, unsupported expressions, and unresolved tokens.
- OpenSpec archive for `p03-template-rendering-engine`.

### Changed

- Application DI now registers `ITemplateRenderer`.
- Template variable listing now reads from the shared variable registry.
- API startup now applies pending EF Core migrations automatically, preventing missing-table failures in local environments.
- Integration tests now verify organizations can be listed without manual database initialization.
- README and architecture documentation now include Phase 3 rendering behavior and supported template variables.

## [0.3.0] - 2026-05-05

### Added

- Sender profile domain model with active/default behavior and sender identity fields.
- Email template domain model with active/inactive lifecycle behavior.
- EF Core mappings, indexes, repositories, and `SenderProfilesAndTemplates` migration.
- Application services and DTOs for sender profile and email template CRUD/deactivation.
- Centralized supported template variable catalog exposed through Application, API, and Web UI.
- REST endpoints and OpenAPI documentation for sender profiles, default sender lookup, email templates, and template variables.
- Blazor pages and typed API clients for sender profile and email template management.
- Domain, application, and integration tests for sender profiles, email templates, template variables, EF persistence, and API endpoints.
- OpenSpec archive for `p02-sender-profiles-and-templates`.

### Changed

- README and architecture documentation now describe configurable sender identity and reusable template management.

## [0.2.0] - 2026-05-05

### Added

- Core contact management model with organizations, contacts, tags, and contact-tag assignments.
- Domain rules for required fields, email validation, contact status changes, do-not-contact behavior, and idempotent tag assignment.
- EF Core SQLite mappings, relational foreign keys, unique indexes, repositories, and `CoreContactsModel` migration.
- Application DTOs and services for organization, contact, and tag CRUD, contact filtering, duplicate prevention, and tag assignment/removal.
- REST endpoints and OpenAPI documentation for organizations, contacts, tags, and contact tag assignments.
- Blazor pages and typed API clients for contacts, organizations, and tags.
- Domain, application, and integration tests for the core contact model, API endpoints, EF persistence, and Web API client error handling.
- OpenSpec archive for `p01-core-contacts-model`.

### Changed

- Web API client error handling now converts non-JSON API errors into controlled UI errors instead of surfacing `JsonException` during rendering.
- Test result directories are ignored recursively to keep coverage output out of commits.
- README and architecture documentation now describe Phase 1 behavior and local API configuration.

## [0.1.0] - 2026-05-05

### Added

- Initial solution setup for layered architecture.
- Domain, Application, Infrastructure, API, Web, and test projects.
- EF Core SQLite configuration and initial migration baseline.
- API health endpoint and Swagger/OpenAPI bootstrap.
- Blazor Web App baseline with initial navigation pages.
- CI workflow with restore, build, and test stages.
- CI coverage reporting for pull requests and main branch builds.
- README badges for CI status and latest main branch coverage.
- Pull request branch validation for `change/*` branches.
- Pull request template aligned with the OpenSpec workflow.
- Manual OpenSpec release workflow with archived change validation.
- Release helper scripts for OpenSpec release validation and changelog extraction.
- Baseline unit and integration tests for project and EF wiring.
- Repository-level development defaults (`.gitignore`, `.editorconfig`, `Directory.Build.props`, `global.json`).
