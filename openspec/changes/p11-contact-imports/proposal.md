## Why

Small teams often start with contacts in spreadsheets. OutreachFlow should import contacts safely from CSV while preventing uncontrolled duplicate creation.

## What Changes

- Add CSV import preview and execution workflow.
- Detect duplicates by normalized email.
- Allow tags to be assigned during import.
- Add `ImportJob` tracking for traceability.
- Add REST endpoints and Blazor import UI.
- Add tests for parsing, preview, duplicate detection, tag assignment, and import persistence.

## Capabilities

### New Capabilities

- `contact-imports`: Controlled CSV contact import with preview, duplicate detection, tags, and import job tracking.

### Modified Capabilities

- None.

## Impact

- Affects Application, Infrastructure, Api, Web, Domain if import job is modeled there, and tests.
- Adds import job persistence and file parsing behavior.
- Depends on contact and tag management.
