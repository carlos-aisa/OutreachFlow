## Context

This follows a product direction review (workflow redesign discussion, validated against a mockup) that reframed the app around campaigns rather than individual contact records. `add-campaign-recipient-lifecycle` was already drafted by treating a `campaign-management` capability as a given; it isn't. This change builds exactly the foundation that proposal assumes, sized so recipient-lifecycle can be implemented directly on top of it without rework.

## Goals / Non-Goals

**Goals:**
- Let a user create a campaign: name, purpose, a message (email template), one or more contact-group audiences, and start it open.
- Let a user list campaigns, open one, see its own data, and close or reopen it.
- Establish the campaign detail screen's basic layout so `add-campaign-recipient-lifecycle` can extend it, not replace it.

**Non-Goals:**
- Recipient tracking, candidate discovery, incorporation, send outcomes, or follow-up linkage — all `add-campaign-recipient-lifecycle`.
- Multiple message versions per campaign — the base campaign references one current template; per-recipient message-version snapshotting at draft-generation time is recipient-lifecycle's concern (already decided in its design).
- Automatic sending of any kind.
- Combining group criteria with AND/OR logic beyond what `contact-group-management` already supports — a campaign's audience is a set of existing groups, not a new criteria language.

## Decisions

**Audience is a set of existing contact groups, not a new selection mechanism.** A campaign references one or more `ContactGroup` ids. This avoids building a second way to define "who," reuses criteria + manual overrides that already exist, and matches what `add-campaign-recipient-lifecycle` already expects for candidate discovery.

**One current message per campaign, not a version history, at this layer.** The campaign holds a mutable reference to one `EmailTemplate`. Changing it going forward affects future draft generation only; it does not rewrite anything already sent. Version snapshotting per recipient (so late joiners can be tracked against the message version they actually received) is deferred entirely to `add-campaign-recipient-lifecycle`'s `MessageVersion` handling — this change does not need to solve that problem.

**Status is a simple Open/Closed flag with no automatic transitions.** A campaign starts open; a user closes or reopens it explicitly. Nothing in this change makes a campaign close itself (e.g., "all recipients sent") — that's a product decision to make later, once recipient-lifecycle exists to observe.

**The campaign detail screen is deliberately incomplete here.** It shows name, status, audience, and message — matching the "info row" of the reviewed mockup — and nothing about recipients, because recipient-lifecycle adds that section (the stat tiles, candidate list, and recipients table) to the same screen. Building a placeholder for that content now would be guessing at an API this change doesn't define.

## Risks / Trade-offs

- Referencing contact groups by id means a group's criteria can change after a campaign starts, silently changing "who counts" → Mitigation: this is intentional and matches the reviewed design — `add-campaign-recipient-lifecycle` explicitly treats criteria matches as candidates, never automatic recipients, so audience drift surfaces as a reviewable suggestion, not a side effect.
- A campaign with zero groups or a template that gets deactivated later → Mitigation: require at least one group at creation; revalidate template/group availability at draft-generation time (recipient-lifecycle's existing design already revalidates do-not-contact, archived status, and similar at that point — template/group activity checks fit the same revalidation step).

## Open Questions

(none outstanding — audience mechanism, message versioning, and status semantics were resolved during the workflow redesign review.)
