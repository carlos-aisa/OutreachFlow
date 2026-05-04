## Why

OutreachFlow needs a generic contact foundation before any template, draft, or email workflow can be useful. This change establishes organizations, contacts, and flexible tags without encoding any business-specific assumptions.

## What Changes

- Add domain model for organizations, contacts, tags, and contact-tag assignments.
- Add EF Core mappings, migration, repositories, and application use cases for basic CRUD and tagging.
- Add REST endpoints for contacts, organizations, tags, and tag assignments.
- Add Blazor screens for listing and creating contacts.
- Add filtering by tag, status, do-not-contact flag, organization, and last contacted date.
- Add domain, application, and integration tests for the contact foundation.
- Update README/OpenAPI documentation for new API contracts.

## Capabilities

### New Capabilities

- `contact-management`: Generic organization, contact, and contact tagging management.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and test projects.
- Adds the first real database tables and EF Core migration.
- Adds initial user-facing contact management API and UI.
