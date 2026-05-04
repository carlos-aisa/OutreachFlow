## Why

OutreachFlow needs reusable documents that can be attached to templates and later adjusted per draft. This change adds attachment asset management without hardcoding any specific file or business content.

## What Changes

- Add attachment asset domain and persistence model.
- Add local file storage abstraction and implementation for MVP.
- Add email template default attachment relationship.
- Add CRUD/upload endpoints and Blazor screens for reusable attachments.
- Allow templates to define default attachments.
- Add tests for file metadata, active state, template attachment mapping, and invalid attachments.

## Capabilities

### New Capabilities

- `attachment-assets`: Reusable attachment assets and default template attachment associations.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Adds file storage configuration and database tables.
- Extends template workflows with default attachment support.
