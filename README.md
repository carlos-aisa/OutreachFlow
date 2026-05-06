# OutreachFlow

[![CI](https://github.com/carlos-aisa/OutreachFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/carlos-aisa/OutreachFlow/actions/workflows/ci.yml)
[![Coverage](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/carlos-aisa/OutreachFlow/main/.github/badges/coverage.json)](https://github.com/carlos-aisa/OutreachFlow/actions/workflows/ci.yml)

OutreachFlow is a lightweight CRM and controlled email outreach manager for small teams and independent professionals. It helps organize contacts, tag audiences, generate personalized email drafts from templates, attach reusable documents, send emails through configurable providers, and track communication history.

## Who It Is For

- Freelancers
- Small businesses
- NGOs
- Consultants
- Trainers and educators
- Associations
- Independent professionals
- Small teams that need controlled outreach

## What Problem It Solves

OutreachFlow centralizes contact and outreach operations in one internal tool with traceability and human approval before sending.

## What It Is Not

OutreachFlow is not a spam engine and is not designed for aggressive bulk mailing.

## Core Features (MVP Target)

- Contacts, organizations, and tags management
- Sender profiles management
- Email templates with validated variables
- Personalized draft generation per contact
- Reusable attachment assets
- Human review and approval flow
- Controlled sending through provider abstraction
- Contact activity history and traceability

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Blazor Web App
- EF Core
- SQLite (MVP)
- xUnit + FluentAssertions
- Swagger / OpenAPI

## Architecture

Layered architecture with strict dependency direction:

- `OutreachFlow.Domain`: entities, value objects, rules, invariants
- `OutreachFlow.Application`: use cases, DTOs, ports/interfaces
- `OutreachFlow.Infrastructure`: EF Core, persistence, migrations, external providers
- `OutreachFlow.Api`: REST endpoints
- `OutreachFlow.Web`: Blazor UI

Detailed baseline: `docs/architecture/ARCHITECTURE.md`.

## Current Status

Phase 0 completed:

- Solution and project structure created
- Layer references configured
- API with Swagger and health endpoint
- Blazor app bootstrapped with basic pages
- EF Core SQLite configured
- Initial migration created
- CI pipeline for restore, build, test

Phase 1 completed:

- Generic organizations, contacts, tags, and contact-tag assignments
- Application services and DTOs for CRUD, filtering, duplicate prevention, and tagging
- EF Core mappings, SQLite migration, unique constraints, and relational foreign keys
- REST endpoints for organizations, contacts, tags, and contact tag assignment
- Blazor pages for contacts, organizations, and tags
- Domain, application, and integration tests for the core contact model

Phase 2 completed:

- Configurable sender profiles with active/default behavior
- Reusable email templates with active/inactive state
- Centralized supported template variable catalog for API and UI guidance
- REST endpoints and Blazor pages for sender profiles and templates
- Domain, application, and integration tests for configurable sender identity and template management

Phase 3 completed:

- `ITemplateRenderer` contract with centralized variable rendering rules
- `TemplateContext` and `RenderedEmail` models for deterministic render diagnostics
- Unknown variable detection and missing value detection for supported variables
- Unresolved token safety checks to block risky draft generation flows
- Application tests for success, unknown variables, missing values, and unsupported expression syntax

Phase 4 completed:

- Reusable attachment assets with active/inactive lifecycle behavior
- Local file storage through `IAttachmentFileStorage` and `LocalAttachmentFileStorage`
- Safe storage root configuration under `AttachmentStorage:RootPath`
- REST endpoints for attachment upload/list/read/update/deactivate
- Template default attachment assignment/removal endpoints
- Blazor attachment management page and template editor attachment controls
- Domain, application, and integration tests for attachment persistence, upload, and association rules

Phase 5 completed:

- `EmailDraft` and `EmailDraftAttachment` models for persisted generated draft snapshots
- Multi-contact draft generation use case with template rendering diagnostics
- Skip reporting for ineligible contacts (for example, Do Not Contact)
- Draft generation APIs (`POST /api/v1/drafts/generate`, list, and detail)
- Blazor draft generation wizard with recipient filters, selections, preview, and result
- Domain, application, and integration tests for draft persistence and generation behavior

Phase 6 completed:

- Draft review workflow with edit, approve, and cancel transitions
- Approval gate that blocks unresolved variables, unresolved diagnostics, and render errors
- Draft review APIs (`PUT /api/v1/drafts/{id}`, `POST /api/v1/drafts/{id}/approve`, `POST /api/v1/drafts/{id}/cancel`)
- Blazor draft list and draft detail pages for review, corrections, approval, and cancellation
- Domain, application, and integration tests for draft review rules and endpoint behavior

Phase 7 completed:

- Email sending abstraction through `IEmailSender` and send contracts (`SendEmailCommand`, `EmailSendResult`)
- Default `FakeEmailSender` provider for local development and automated tests
- Controlled send use case for approved drafts with validation rules:
  - approved-only sending,
  - do-not-contact enforcement,
  - unresolved-variable blocking,
  - active attachment validation,
  - duplicate and equivalent recent send prevention
- `EmailMessage` persistence for successful and failed send attempts
- Draft send status transitions (`Sent`, `Failed`) with `sentAt` and failure reason tracking
- Contact `LastContactedAt` update on successful send
- Send endpoint and UI action (`POST /api/v1/drafts/{id}/send`)

Phase 8 completed:

- `ContactActivity` domain model and persisted timeline events linked to contacts
- Automatic activity recording for:
  - contact create/update,
  - status transitions,
  - draft generation,
  - email send success/failure
- Contact activity endpoint (`GET /api/v1/contacts/{id}/activities`)
- Contact detail page with activity timeline in Blazor
- Domain, application, and integration tests for activity persistence and retrieval order

Phase 9 completed:

- Real SMTP provider support (`SmtpEmailSender`) behind `IEmailSender`
- Provider selection and required SMTP configuration validation when `EmailSending:Provider=SMTP`
- SMTP setup guidance with user secrets and placeholder configuration values
- Integration tests for SMTP option validation, provider selection, failure mapping, and fake provider enforcement in API integration tests

Phase 10 completed:

- `FollowUpTask` domain model with completion lifecycle (`IsCompleted`, `CompletedAt`)
- Follow-up task CRUD and completion APIs
- Follow-up management page in Blazor (`/follow-ups`)
- Pending follow-up visibility on dashboard and contact detail
- Optional automatic post-send follow-up creation via `FollowUpAutomation` configuration
- Follow-up activity tracking (`FollowUpCreated`, `FollowUpCompleted`)

## Roadmap

- Phase 1: Core contacts model (organizations, contacts, tags)
- Phase 2: Sender profiles and templates
- Phase 3: Template rendering engine
- Phase 4: Attachment assets
- Phase 5: Draft generation
- Phase 6: Draft review and approval
- Phase 7: Controlled sending abstraction
- Phase 8: Contact activity history
- Phase 9: SMTP real provider
- Phase 10: Follow-up tasks
- Phase 11+: Imports and future integrations

## Run Locally

1. Restore dependencies:

```bash
dotnet restore
```

2. Optional: apply migrations manually (the API also applies pending migrations on startup):

```bash
dotnet ef database update --project src/OutreachFlow.Infrastructure --startup-project src/OutreachFlow.Api
```

3. Optional: configure local attachment storage root (default: `storage/attachments`):

```json
{
  "AttachmentStorage": {
    "RootPath": "storage/attachments"
  }
}
```

4. Optional: configure SMTP sender (use secrets, never commit credentials):

```bash
dotnet user-secrets set "EmailSending:Provider" "SMTP" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Host" "smtp.example.com" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Port" "587" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:UseSsl" "true" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Username" "your-smtp-user" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Password" "your-smtp-password" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:TimeoutSeconds" "30" --project src/OutreachFlow.Api
```

5. Optional: configure automatic follow-up creation after successful sends:

```bash
dotnet user-secrets set "FollowUpAutomation:AutoCreateAfterSuccessfulSend" "true" --project src/OutreachFlow.Api
dotnet user-secrets set "FollowUpAutomation:DueDaysAfterSend" "7" --project src/OutreachFlow.Api
dotnet user-secrets set "FollowUpAutomation:DefaultType" "Email" --project src/OutreachFlow.Api
```

6. Run API:

```bash
dotnet run --project src/OutreachFlow.Api
```

7. Run Web UI:

```bash
dotnet run --project src/OutreachFlow.Web
```

The Web UI calls the API configured by `OutreachFlowApi:BaseUrl`. The default local value is `http://localhost:5131`, matching the API `http` launch profile.

## API Surface

The v1 OpenAPI contract is maintained in `docs/api/openapi.v1.yaml`. Current endpoints include:

- `/api/v1/organizations`
- `/api/v1/contacts`
- `/api/v1/contacts/{id}/tags/{tagId}`
- `/api/v1/contacts/{id}/activities`
- `/api/v1/tags`
- `/api/v1/sender-profiles`
- `/api/v1/sender-profiles/default`
- `/api/v1/templates`
- `/api/v1/templates/variables`
- `/api/v1/templates/{id}/attachments/{attachmentId}`
- `/api/v1/attachments`
- `/api/v1/attachments/{id}`
- `/api/v1/drafts/generate`
- `/api/v1/drafts`
- `/api/v1/drafts/{id}`
- `/api/v1/drafts/{id}` (PUT for edit)
- `/api/v1/drafts/{id}/approve`
- `/api/v1/drafts/{id}/cancel`
- `/api/v1/drafts/{id}/send`
- `/api/v1/follow-ups`
- `/api/v1/follow-ups/{id}`
- `/api/v1/follow-ups/{id}/complete`

## Supported Template Variables

Supported variable names are centralized and validated:

- `contact.displayName`
- `contact.email`
- `contact.role`
- `organization.name`
- `organization.city`
- `organization.province`
- `sender.name`
- `sender.email`
- `sender.phone`
- `sender.organizationName`
- `sender.website`
- `sender.signature`

Rendering supports only direct substitution with `{{variable.name}}` syntax. Loops, conditionals, scripts, and expression evaluation are intentionally out of scope for MVP.

## Run Tests

```bash
dotnet test
```

## GitHub Workflows

- Pull requests run restore, build, tests, and total coverage reporting.
- Coverage is reported in the workflow summary, uploaded as an HTML artifact, and posted as a pull request comment.
- The README coverage badge is updated from the latest successful `main` coverage run.
- Coverage is report-only for now and does not block pull requests.
- Pull requests must be opened from branches named `change/<change-id-or-short-description>`.
- Completed OpenSpec changes are released through the manual `release openspec change` workflow after the change is archived, `VERSION` is bumped, and `CHANGELOG.md` has a matching entry.

## OpenSpec Branch Workflow

Each OpenSpec change must be implemented in its own branch and reviewed through a pull request.

```bash
git switch main
git pull
git switch -c change/p01-core-contacts-model
```

Use one branch per change. Do not implement multiple OpenSpec changes in the same pull request.

## Release Process

1. Implement one OpenSpec change on a branch.
2. Mark all tasks complete in `tasks.md`.
3. Archive the change with `openspec archive <change-id>`.
4. Update `VERSION` and add a matching `CHANGELOG.md` section.
5. Merge the pull request to `main` after CI passes.
6. Run the manual release workflow with the archived change id and SemVer version.

## Technical Notes

- Provider-specific email sending is abstracted for future SMTP/Gmail/Graph implementations.
- The default provider is `Fake` (`EmailSending:Provider`).
- SMTP is supported for local/manual configuration (`EmailSending:Provider=SMTP`) with settings under `EmailSending:Smtp:*`.
- SMTP configuration requires `Host`, `Port`, `Username`, `Password`, and positive `TimeoutSeconds`.
- Fake sender failures can be simulated using `EmailSending:FakeFailureKeyword` in subject/body/recipient (default: `[fail-send]`).
- SMTP delivery and provider-level throttling/reputation behavior depend on your external provider and are intentionally outside the app logic.
- Follow-up auto creation after send is opt-in (`FollowUpAutomation:AutoCreateAfterSuccessfulSend=false` by default).
- Secrets must be managed through environment variables or user secrets, never in source control.
- MVP focus is controlled and traceable outreach, not automation at scale.
