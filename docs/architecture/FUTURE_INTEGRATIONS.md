# Future Integrations Foundation

## Purpose

This document defines the boundaries for post-MVP integrations without changing current runtime behavior.

## Current MVP Baseline

- Contact data source of truth remains OutreachFlow internal database.
- Email providers currently supported:
  - `Fake` (default)
  - `SMTP` (opt-in)
- Database runtime provider remains SQLite.
- Background jobs are not enabled.
- OpenTelemetry is not enabled.

## Provider Boundaries

### Email Providers

Future providers (`Gmail API`, `Microsoft Graph`) must:

- implement `IEmailSender` in Infrastructure,
- stay provider-agnostic at Domain/Application layers,
- be disabled by default unless explicitly configured.

### Contact Sources and Sync

Future imports (`Google Contacts`, `Outlook Contacts`) must:

- start as one-way imports unless explicitly expanded by a dedicated OpenSpec change,
- preserve OutreachFlow tags and business metadata inside OutreachFlow,
- avoid automatic write-back to external systems in early versions.

## External System Safety Rule

OutreachFlow must not automatically modify Gmail, Outlook, Google Contacts, or Outlook Contacts records in early versions.

Any bidirectional sync or external write-back requires:

- a dedicated OpenSpec change,
- explicit approval,
- dedicated tests and rollback strategy.

## Infrastructure Readiness (Post-MVP)

The following options are planned, but not enabled in current runtime:

- PostgreSQL profile
- Docker Compose local stack
- Background jobs (Hangfire or Quartz.NET)
- OpenTelemetry tracing/metrics

Each option requires a separate implementation OpenSpec change.

## Configuration Defaults

Current configuration keeps future providers disabled by default:

- `EmailSending:GmailApi:Enabled = false`
- `EmailSending:MicrosoftGraph:Enabled = false`
- `ContactSync:GoogleContacts:Enabled = false`
- `ContactSync:OutlookContacts:Enabled = false`

These placeholders are documentation-oriented and do not enable runtime integration by themselves.
