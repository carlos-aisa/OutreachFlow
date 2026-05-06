# OutreachFlow

[![CI](https://github.com/carlos-aisa/OutreachFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/carlos-aisa/OutreachFlow/actions/workflows/ci.yml)
[![Coverage](https://img.shields.io/endpoint?url=https://raw.githubusercontent.com/carlos-aisa/OutreachFlow/main/.github/badges/coverage.json)](https://github.com/carlos-aisa/OutreachFlow/actions/workflows/ci.yml)
[![Latest Release](https://img.shields.io/github/v/release/carlos-aisa/OutreachFlow?sort=semver)](https://github.com/carlos-aisa/OutreachFlow/releases)
[![.NET](https://img.shields.io/badge/.NET-8-512BD4)](https://dotnet.microsoft.com/)
[![OpenAPI](https://img.shields.io/badge/OpenAPI-v1-6BA539)](docs/api/openapi.v1.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

OutreachFlow is a lightweight CRM and controlled email outreach manager for small teams and independent professionals. It helps organize contacts, classify audiences with flexible tags, generate personalized drafts from reusable templates, attach reusable assets, send through configurable providers, and keep a complete communication history.

Current stable line: `v0.13.0` ([CHANGELOG](CHANGELOG.md), [Releases](https://github.com/carlos-aisa/OutreachFlow/releases)).

## Table of Contents

- [Why OutreachFlow](#why-outreachflow)
- [What It Is Not](#what-it-is-not)
- [Project Highlights](#project-highlights)
- [Feature Scope](#feature-scope)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Repository Structure](#repository-structure)
- [API and Contracts](#api-and-contracts)
- [Local Setup](#local-setup)
- [Quality and CI](#quality-and-ci)
- [Testing Matrix](#testing-matrix)
- [Release Strategy](#release-strategy)
- [Documentation Map](#documentation-map)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [Security](#security)

## Why OutreachFlow

Many small organizations need outreach discipline but not enterprise CRM complexity. OutreachFlow focuses on a practical middle ground:

- Contact-centric workflow with full traceability
- Human approval before send
- Provider abstraction (`Fake`, `SMTP`, and future providers)
- Rules that block unsafe sending (Do Not Contact, unresolved variables, duplicate/equivalent recent send protection)

### Who It Is For

- Freelancers
- Small businesses
- NGOs
- Consultants
- Trainers and educators
- Associations
- Independent professionals
- Small teams needing controlled external communication

## What It Is Not

OutreachFlow is not a spam engine and is not designed for aggressive bulk mailing, scraping, or invasive automation.

## Project Highlights

- Clean layered architecture with strict dependency direction
- Explicit business rules in Domain and Application layers
- Template rendering engine with unknown/missing variable diagnostics
- Controlled draft lifecycle: generate, review, approve, send, audit
- Provider abstraction and swap-ready infrastructure design
- Rich automated test suite across domain, application, and integration levels
- OpenSpec-driven delivery with one-change-per-branch workflow
- CI with coverage reporting visible in PRs and README badge
- Manual, auditable release process based on archived OpenSpec changes

## Feature Scope

### Completed

- Organizations, contacts, tags, and status management
- Sender profiles and email templates with variable catalog
- Sender profile signatures (HTML/RTF) with validation and draft-body append behavior
- Attachment assets and template default attachments
- Draft generation, review, approval, cancellation, and controlled send
- Activity timeline (contact and email events)
- SMTP provider support (configuration-based)
- Follow-up tasks with optional post-send automation
- CSV contact imports with preview, duplicate detection, tag assignment, and import job tracking
- Release installer packaging with versioned Windows installer assets in GitHub Releases
- Spanish localization for navigation and core contact/template management workflows

### Next

- Future providers (Gmail API, Microsoft Graph)
- Queue and throttling controls
- PostgreSQL support profile

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Blazor Web App
- EF Core + SQLite
- xUnit + FluentAssertions
- Swagger/OpenAPI
- GitHub Actions (CI, coverage, release workflow)

## Architecture

- `OutreachFlow.Domain`: entities, value objects, business invariants
- `OutreachFlow.Application`: use cases, DTOs, service contracts, orchestration rules
- `OutreachFlow.Infrastructure`: EF Core persistence, repositories, migrations, provider implementations
- `OutreachFlow.Api`: REST endpoints and API composition
- `OutreachFlow.Web`: Blazor Web App management UI

Detailed architecture document: [docs/architecture/ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md)

## Repository Structure

```text
OutreachFlow/
|- src/
|  |- OutreachFlow.Domain/
|  |- OutreachFlow.Application/
|  |- OutreachFlow.Infrastructure/
|  |- OutreachFlow.Api/
|  |- OutreachFlow.Web/
|- tests/
|  |- OutreachFlow.Domain.Tests/
|  |- OutreachFlow.Application.Tests/
|  |- OutreachFlow.IntegrationTests/
|- docs/
|  |- api/openapi.v1.yaml
|  |- architecture/ARCHITECTURE.md
|  |- standards/
|- openspec/
|- scripts/
|- CHANGELOG.md
|- VERSION
|- README.md
```

## API and Contracts

- OpenAPI contract: [docs/api/openapi.v1.yaml](docs/api/openapi.v1.yaml)
- Swagger UI is enabled in local API runs
- API uses `/api/v1/*` versioned routes

Current endpoint groups:

- Organizations
- Contacts, contact tags, and activities
- Tags
- Sender profiles
- Templates and template attachments
- Attachment assets
- Draft generation, review, approval, cancellation, and send
- Follow-up tasks

## Local Setup

### Prerequisites

- .NET SDK 8.x
- Git

### Run

1. Restore dependencies.

```bash
dotnet restore
```

2. Apply migrations (optional; API also applies pending migrations on startup).

```bash
dotnet ef database update --project src/OutreachFlow.Infrastructure --startup-project src/OutreachFlow.Api
```

3. Run API.

```bash
dotnet run --project src/OutreachFlow.Api
```

4. Run Web UI.

```bash
dotnet run --project src/OutreachFlow.Web
```

Default local API base URL expected by Web: `http://localhost:5131`.

### Optional SMTP Configuration (Local Only)

```bash
dotnet user-secrets set "EmailSending:Provider" "SMTP" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Host" "smtp.example.com" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Port" "587" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:UseSsl" "true" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Username" "your-user" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:Password" "your-password" --project src/OutreachFlow.Api
dotnet user-secrets set "EmailSending:Smtp:TimeoutSeconds" "30" --project src/OutreachFlow.Api
```

### Optional Follow-up Automation

```bash
dotnet user-secrets set "FollowUpAutomation:AutoCreateAfterSuccessfulSend" "true" --project src/OutreachFlow.Api
dotnet user-secrets set "FollowUpAutomation:DueDaysAfterSend" "7" --project src/OutreachFlow.Api
dotnet user-secrets set "FollowUpAutomation:DefaultType" "Email" --project src/OutreachFlow.Api
```

## Quality and CI

### Tests

```bash
dotnet test
```

### Pull Request CI

- `dotnet restore`
- `dotnet build --configuration Release --no-restore`
- `dotnet test --configuration Release --no-build --collect:"XPlat Code Coverage"`
- Merged coverage summary via ReportGenerator
- Sticky PR comment with total coverage
- HTML coverage report artifact upload
- Coverage badge refresh on `main`

Coverage is report-only (no threshold gate yet).

### Engineering Quality Gates

| Area | Mechanism | Signal |
| --- | --- | --- |
| Build and tests | `.github/workflows/ci.yml` | Required quality baseline for PRs |
| Coverage visibility | ReportGenerator summary + sticky PR comment + README badge | Continuous visibility without hard gate |
| Release control | `.github/workflows/release-openspec-change.yml` | Manual, auditable SemVer releases from archived OpenSpec changes |
| Branch governance | `change/<change-id-or-short-description>` validation in CI | One change per branch and review-ready traceability |

## Testing Matrix

Run all automated tests:

```bash
dotnet test
```

Run by suite:

```bash
dotnet test tests/OutreachFlow.Domain.Tests
dotnet test tests/OutreachFlow.Application.Tests
dotnet test tests/OutreachFlow.IntegrationTests
```

## CSV Import Format

OutreachFlow CSV imports are review-first: preview always runs before commit.

- Required headers: `displayName`, `email`
- Optional headers: `phone`, `role`, `source`
- Encoding: UTF-8 text CSV
- Duplicate detection: normalized email against current database contacts and repeated emails in the same CSV file
- Commit behavior: only valid non-duplicate rows are created
- Tag assignment: selected existing tag ids are assigned to each imported contact
- Scope limits: no Excel import, no Google/Outlook contacts sync, and no background batch processing in current phase

## Release Strategy

Releases are manual and controlled after an OpenSpec change is completed.

1. Implement one OpenSpec change in `change/<id>` branch.
2. Complete tasks in `tasks.md`.
3. Archive change under `openspec/changes/archive/`.
4. Bump `VERSION` and add matching `CHANGELOG.md` section.
5. Merge to `main` after CI passes.
6. Run `release-openspec-change` workflow with `change_id` and `version`.
7. Download installer asset `OutreachFlow-v<version>-win-x64-installer.zip` from the release page.

Versioning follows Semantic Versioning (`vX.Y.Z`).

## Documentation Map

- Architecture baseline: [docs/architecture/ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md)
- OpenAPI contract: [docs/api/openapi.v1.yaml](docs/api/openapi.v1.yaml)
- Engineering standards: [docs/standards](docs/standards)
- Future integration boundaries: [docs/architecture/FUTURE_INTEGRATIONS.md](docs/architecture/FUTURE_INTEGRATIONS.md)
- Installer packaging details: [docs/release/INSTALLER_RELEASE.md](docs/release/INSTALLER_RELEASE.md)
- Localization setup and translation keys: [docs/localization/LOCALIZATION.md](docs/localization/LOCALIZATION.md)
- Change proposals and archived delivery history: [openspec](openspec)
- Release history: [CHANGELOG.md](CHANGELOG.md)

## Roadmap

- Phase 12: Future integration foundation (boundaries, safety rules, and disabled-by-default configuration placeholders)
- Phase 16+: Contact/provider integrations, queueing, observability, PostgreSQL profile

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for branch naming, PR expectations, and quality checks.

## Security

See [SECURITY.md](SECURITY.md) for responsible disclosure and secret-handling expectations.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
