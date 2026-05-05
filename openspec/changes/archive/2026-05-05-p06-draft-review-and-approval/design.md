## Context

Draft generation creates draft records, but users still need to review and correct them. This phase turns drafts into an explicit approval workflow.

## Goals / Non-Goals

**Goals:**

- Let users view, edit, approve, and cancel drafts.
- Prevent approval when render errors remain or unresolved variables exist.
- Preserve timestamps for approval and cancellation states.
- Provide a draft list filtered by status.

**Non-Goals:**

- Sending drafts.
- Re-rendering templates after edits.
- Collaborative review or audit comments.

## Decisions

- Approval is a domain state transition from `Draft` or corrected `NeedsReview` to `Approved`.
- Manual edits update subject/body snapshots and clear render diagnostics only after validation confirms no unresolved variables remain.
- Cancelled drafts remain stored for traceability rather than being deleted.
- The UI detail page exposes final recipient, subject, body, attachments, diagnostics, and status actions.

## Risks / Trade-offs

- Clearing diagnostics after manual edits can hide original render problems. Mitigation: keep initial diagnostics fields until the user saves a corrected draft with no unresolved tokens.
- Approval rules may grow with sending rules. Mitigation: keep approval and sending validation separate.

## Migration Plan

- Reuse existing draft table when possible.
- Add columns only if current draft schema lacks required review metadata.
- Update endpoints, OpenAPI, and tests.

## Open Questions

- None for MVP.
