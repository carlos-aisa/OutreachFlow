## Context

An active campaign must accommodate contacts that become eligible after its initial mailout. Existing drafts and sends are contact-level records with no campaign ownership or campaign-recipient state.

## Goals / Non-Goals

**Goals:**
- Track recipient inclusion and delivery state independently for each campaign.
- Detect eligible contacts not yet incorporated into an open campaign.
- Prevent duplicates for the same campaign and message version while retaining human approval.

**Non-Goals:**
- Automatic sending, campaign analytics, or arbitrary version branching.

## Decisions

- Add a campaign-recipient join entity as the authoritative recipient history. Do not infer history only from drafts because cancelled, skipped, and candidate states must be retained.
- Add explicit incorporation action; detected candidates are suggestions, never automatic recipients or sends.
- Assign a message version to campaign recipients when drafts are generated. Future message-edit behavior remains constrained to new drafts; sent drafts remain immutable.
- Reuse existing EmailDraft approval and send safety rules, with campaign recipient state updated transactionally from outcomes.

## Risks / Trade-offs

- [Concurrent incorporation creates duplicates] → Enforce a unique relational constraint per campaign, contact, and message version.
- [Eligibility changes after inclusion] → Revalidate do-not-contact, archived status, email validity, sender, and attachments before generation and sending.
- [State becomes inconsistent after send failure] → Update recipient state from persisted send outcomes and test failure paths with relational integration tests.
