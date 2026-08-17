## Context

Current bulk draft generation uses a mandatory wizard and separates message, audiences, drafts, sending, and follow-ups across technical screens. Periodic outreach needs a persistent work item that can be prepared in the order that makes sense to the user.

## Goals / Non-Goals

**Goals:**
- Store campaigns in preparation and expose their readiness clearly.
- Let message, audience, sender, attachments, and follow-up configuration be edited independently.
- Integrate groups as one campaign-audience source.

**Non-Goals:**
- Automatically send mail, implement analytics, or implement late-recipient processing (that is the recipient lifecycle proposal).

## Decisions

- Introduce Campaign as the aggregate that owns its preparation data; existing EmailDraft remains the per-contact review unit.
- Save campaign data incrementally and enforce completeness only at generate/send boundaries. A wizard would again impose an artificial order.
- Surface campaigns in primary navigation and dashboard queues; retain templates, sender profiles, attachments, and tags as configuration destinations.
- Campaign audiences reference groups and/or explicit contacts, but generated recipients are handled by the later lifecycle proposal.

## Risks / Trade-offs

- [Partially complete campaigns linger] → Use explicit preparation status and readiness checklist.
- [Audience changes are confusing] → Display selected sources and counts; recipient snapshots are deferred to lifecycle work.
- [Large UI change] → Deliver campaign workspace independently after groups and preserve current draft flows during transition.
