## Context

Contacts exist after Phase 1, but outreach must not contain hardcoded sender details or business-specific copy. Sender profiles and templates provide the configurable content layer before rendering is introduced.

## Goals / Non-Goals

**Goals:**

- Manage sender profiles with optional default profile behavior.
- Manage active/inactive email templates with subject and body templates.
- Expose supported variable names for UI guidance.
- Keep template rendering out of this phase.

**Non-Goals:**

- Rendering or validating variable values.
- Draft generation or sending.
- Rich text editing, attachment management, or provider configuration.

## Decisions

- Sender profiles are aggregate roots because they have lifecycle rules, including default selection.
- Email templates are aggregate roots because drafts and attachments will reference them later.
- The available variable list lives in Application as a contract so API and Web can display the same guidance.
- Only one sender profile can be default at a time, enforced by application logic and verified in tests.

## Risks / Trade-offs

- Default sender behavior can become subtle as multiple users are introduced. Mitigation: keep MVP single-tenant and document the assumption.
- Templates may contain invalid variables before the renderer exists. Mitigation: expose supported variables now and enforce validation in Phase 3.

## Migration Plan

- Add sender profile and email template tables.
- Add migration and integration tests.
- Update OpenAPI documentation.

## Open Questions

- None for MVP.
