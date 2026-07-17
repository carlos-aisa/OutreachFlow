## Context

Contacts and organizations are managed on separate Blazor pages. Although the domain permits contacts without an organization, the current entry sequence implies that an organization must be created first. The primary user is non-technical and enters contacts frequently.

## Goals / Non-Goals

**Goals:**
- Provide one contact-first entry surface with optional existing-organization selection or inline organization details.
- Persist a new organization and its contact atomically when both are supplied.
- Direct the user to the most useful next action after saving.

**Non-Goals:**
- Redesign group or campaign management.
- Change contact uniqueness, organization fields, or existing contact CRUD behavior.

## Decisions

- Add a dedicated application command for combined organization/contact creation rather than coordinating two API calls in the browser. This preserves atomicity; sequential client calls could leave an unused organization after a contact validation failure.
- Keep the existing organizations page for administration, while making inline organization entry an optional branch of contact intake.
- Use progressive disclosure for optional fields and a contextual post-save action; do not introduce a blocking multi-step wizard.

## Risks / Trade-offs

- [Inline form becomes too dense] → Show only contact essentials initially and reveal optional organization details on demand.
- [Duplicate organization names] → Reuse existing organization validation and present existing organization selection before new-organization entry.
- [New API contract diverges from CRUD] → Document it in OpenAPI and cover it with API tests.
