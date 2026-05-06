# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog and this project follows Semantic Versioning.

## [0.13.0] - 2026-05-06

### Added

- Future integrations foundation documentation:
  - `docs/architecture/FUTURE_INTEGRATIONS.md`
  - architecture section updates for provider boundaries and post-MVP options
- Configuration placeholders (disabled by default) for future integrations:
  - `EmailSending:GmailApi:Enabled`
  - `EmailSending:MicrosoftGraph:Enabled`
  - `ContactSync:GoogleContacts:Enabled`
  - `ContactSync:OutlookContacts:Enabled`
- OpenSpec archive for `p12-future-integrations-foundation`.

### Changed

- README roadmap and documentation map now reference the future integration foundation.
- Explicitly documented safety rule: no automatic modification of external contact systems in early versions.

## [0.12.0] - 2026-05-06

### Added

- Contact import domain support:
  - `ImportJob`
  - `ImportJobStatus`
- CSV import application services and contracts:
  - `IContactImportService`
  - `ContactImportService`
  - `IContactImportCsvParser`
  - `StructuredCsvContactImportParser`
  - `IImportJobRepository`
- Contact import persistence:
  - `ImportJobs` table
  - EF Core migration `ContactImports`
  - `ImportJobRepository`
- Contact import API endpoints:
  - `POST /api/v1/contact-imports/preview`
  - `POST /api/v1/contact-imports/commit`
  - `GET /api/v1/contact-imports/jobs`
- Blazor imports page (`/imports`) with:
  - CSV upload,
  - row-level preview diagnostics,
  - tag selection,
  - commit summary,
  - recent import jobs list.
- Domain, application, and integration tests for import parsing, validation, duplicate detection, tag assignment, job tracking, and API behavior.
- OpenSpec archive for `p11-contact-imports`.

### Changed

- OpenAPI contract updated to `0.12.0` with contact import schemas and endpoints.
- README and architecture docs updated with CSV import format, behavior, and limitations.

## [0.11.0] - 2026-05-06

### Added

- Follow-up task domain support:
  - `FollowUpTask`
  - `FollowUpTaskType`
- Follow-up task application contracts and services:
  - `IFollowUpTaskRepository`
  - `IFollowUpTaskService`
  - `FollowUpTaskService`
- Optional follow-up automation policy for post-send task creation:
  - `IFollowUpAutomationPolicy`
  - `ConfiguredFollowUpAutomationPolicy`
- Follow-up persistence table and migration (`FollowUpTasks`) with contact/due-date indexes.
- Follow-up API endpoints:
  - `GET /api/v1/follow-ups`
  - `POST /api/v1/follow-ups`
  - `GET /api/v1/follow-ups/{id}`
  - `PUT /api/v1/follow-ups/{id}`
  - `POST /api/v1/follow-ups/{id}/complete`
- Blazor follow-up page (`/follow-ups`) with creation, listing, and completion actions.
- Dashboard pending follow-up summary and contact detail follow-up section.
- Follow-up activity timeline events (`FollowUpCreated`, `FollowUpCompleted`).
- Domain, application, and integration tests for follow-up CRUD/completion, filtering/order, and API behavior.
- OpenSpec archive for `p10-follow-up-tasks`.

### Changed

- Successful draft sends can optionally auto-create follow-up tasks through `FollowUpAutomation` settings.
- README, architecture docs, and OpenAPI now document follow-up behavior and configuration.

## [0.10.0] - 2026-05-06

### Added

- Real SMTP provider support through `SmtpEmailSender` behind `IEmailSender`.
- SMTP configuration model and validator:
  - `EmailSending:Smtp:Host`
  - `EmailSending:Smtp:Port`
  - `EmailSending:Smtp:UseSsl`
  - `EmailSending:Smtp:Username`
  - `EmailSending:Smtp:Password`
  - `EmailSending:Smtp:TimeoutSeconds`
- Conditional provider resolution in Infrastructure DI for `Fake` and `SMTP`.
- SMTP transport abstraction and system transport factory for network send execution.
- Integration tests for:
  - SMTP option validation,
  - provider selection,
  - SMTP failure mapping without real network calls,
  - API integration factory staying on `FakeEmailSender`.
- OpenSpec archive for `p09-real-email-provider-smtp`.

### Changed

- API appsettings now include placeholder SMTP configuration keys (no secrets).
- README and architecture docs now include SMTP setup and limitations guidance.

## [0.9.0] - 2026-05-06

### Added

- `ContactActivity` domain model and `ContactActivityType` enum for contact timeline events.
- Application activity services/contracts:
  - `IContactActivityRepository`
  - `IContactActivityService`
  - `ContactActivityService`
- Automatic activity recording in core flows:
  - contact creation/update/status transitions,
  - draft generation,
  - draft send success/failure.
- EF Core `ContactActivityHistory` migration with indexes and foreign keys.
- Contact activity API endpoint: `GET /api/v1/contacts/{id}/activities`.
- Blazor contact detail page with activity timeline view.
- Domain, application, persistence, and API integration tests for activity history behavior.
- OpenSpec archive for `p08-contact-activity-history`.

### Changed

- OpenAPI and README now include contact activity history and traceability surface.

## [0.8.0] - 2026-05-05

### Added

- Email sending abstraction contracts in Application:
  - `IEmailSender`
  - `SendEmailCommand` and payload models
  - `EmailSendResult`
- `EmailMessage` domain model and persistence for send attempt traceability.
- `FakeEmailSender` Infrastructure provider with configurable failure keyword simulation.
- Configurable email sending policy (`EquivalentEmailWindowHours`) for equivalent recent send prevention.
- `SendApprovedDraftAsync` use case with controlled validation rules:
  - approved-only sends,
  - do-not-contact block,
  - unresolved variable/diagnostic block,
  - active attachment validation,
  - duplicate/equivalent recent send prevention.
- Send API endpoint: `POST /api/v1/drafts/{id}/send`.
- Draft send UI action in review page.
- Domain, application, integration, and persistence tests for sending success/failure and duplicate prevention.
- OpenSpec archive for `p07-email-sending-abstraction`.

### Changed

- `EmailDraft` now stores send outcome metadata (`sentAt`, `failureReason`) and supports `Sent`/`Failed` transitions.
- OpenAPI, README, and architecture documentation now describe the Phase 7 controlled sending workflow and fake provider behavior.

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
