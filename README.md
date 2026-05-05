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

## Roadmap

- Phase 1: Core contacts model (organizations, contacts, tags)
- Phase 2: Sender profiles and templates
- Phase 3: Template rendering engine
- Phase 4: Attachment assets
- Phase 5: Draft generation
- Phase 6: Draft review and approval
- Phase 7: Controlled sending abstraction
- Phase 8: Contact activity history
- Phase 9+: SMTP and future integrations

## Run Locally

1. Restore dependencies:

```bash
dotnet restore
```

2. Apply migrations:

```bash
dotnet ef database update --project src/OutreachFlow.Infrastructure --startup-project src/OutreachFlow.Api
```

3. Run API:

```bash
dotnet run --project src/OutreachFlow.Api
```

4. Run Web UI:

```bash
dotnet run --project src/OutreachFlow.Web
```

The Web UI calls the API configured by `OutreachFlowApi:BaseUrl`. The default local value is `http://localhost:5131`, matching the API `http` launch profile.

## API Surface

The v1 OpenAPI contract is maintained in `docs/api/openapi.v1.yaml`. Phase 1 adds:

- `/api/v1/organizations`
- `/api/v1/contacts`
- `/api/v1/contacts/{id}/tags/{tagId}`
- `/api/v1/tags`
- `/api/v1/sender-profiles`
- `/api/v1/sender-profiles/default`
- `/api/v1/templates`
- `/api/v1/templates/variables`

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
- Secrets must be managed through environment variables or user secrets, never in source control.
- MVP focus is controlled and traceable outreach, not automation at scale.
