## Context

Templates exist after Phase 2, but reusable attachments are still missing. The MVP uses local file storage while keeping storage behind an application port for future provider changes.

## Goals / Non-Goals

**Goals:**

- Register and upload reusable attachment assets.
- Store file metadata and local storage path.
- Support active/inactive attachment state.
- Associate default attachments with email templates.

**Non-Goals:**

- Draft-specific attachment selection.
- Cloud storage providers.
- Virus scanning or document preview.
- Business-specific document naming or content.

## Decisions

- Store binary files outside the database and metadata in EF Core. This keeps SQLite lightweight.
- Define file storage as an application port implemented in Infrastructure.
- Use an `EmailTemplateAttachment` join table for default attachments.
- Validate attachment active state when associating assets with templates.

## Risks / Trade-offs

- Local file storage needs careful path handling. Mitigation: store files under a configured root and never trust user-provided paths.
- File cleanup can become complex. Mitigation: start with deactivation and metadata tracking; physical deletion can be explicit.

## Migration Plan

- Add attachment and template attachment tables.
- Add storage root configuration with safe development default.
- Add migration, API docs, and integration tests.

## Open Questions

- Maximum file size can start with a conservative configurable default.
