## Context

After attachments and rendering exist, the next workflow is generating drafts in a controlled way. Drafts must preserve rendered content and diagnostics so users can review concrete emails before approval.

## Goals / Non-Goals

**Goals:**

- Generate personalized drafts for selected contacts.
- Store rendered subject/body and render diagnostics.
- Attach default and selected active attachments to drafts.
- Prevent draft generation for contacts that are not eligible for outreach.

**Non-Goals:**

- Sending drafts.
- Editing, approving, or cancelling drafts.
- Background queues or rate limiting.

## Decisions

- Persist rendered subject and body in `EmailDraft` so later edits and sends are traceable.
- Keep draft status as a domain enum with `Draft` and `NeedsReview` generated in this phase.
- Store render diagnostics on the draft so the review screen can explain issues without re-rendering.
- Validate attachment active state during generation to avoid stale or unavailable assets.

## Risks / Trade-offs

- Stored rendered content can become stale if a template changes. Mitigation: drafts are snapshots by design.
- Generating many drafts in one request could be slow. Mitigation: MVP remains controlled and modest; background generation can come later.

## Migration Plan

- Add draft and draft attachment tables.
- Add use case and endpoints.
- Add UI wizard after API behavior is stable.

## Open Questions

- The first implementation can use synchronous request/response generation with clear limits.
