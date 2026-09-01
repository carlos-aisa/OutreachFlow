## Why

Campaigns remain useful after an initial mailing because suitable contacts may be added later. The system needs recipient-level history and controlled additions so users can reuse an active campaign without accidental duplicate sends or automatic outreach.

## What Changes

- Add campaign recipient records and lifecycle states for candidates, drafts, approvals, sends, exclusions, and failures.
- Let users find and explicitly incorporate eligible, previously unsent contacts into an open campaign.
- Prevent a campaign from generating or sending duplicate outreach to the same contact for the same message version.
- Preserve the existing human review and sending safety rules for every newly incorporated recipient.
- Link campaign follow-up configuration and resulting work to recipient delivery outcomes.

## Capabilities

### New Capabilities
- `campaign-recipient-lifecycle`: Recipient discovery, inclusion, state tracking, deduplication, and late-joiner handling for campaigns.

### Modified Capabilities
- `email-draft-generation`: Generate and report drafts against campaign recipients.
- `email-sending`: Persist campaign context for send attempts and maintain recipient delivery state.
- `follow-up-tasks`: Associate campaign-configured post-send follow-ups with campaign recipients.

## Impact

- Adds recipient lifecycle domain and relational persistence, campaign-aware application/API/OpenAPI contracts, and campaign workspace screens.
- Depends on `campaign-management` and benefits from `contact-group-management` for candidate discovery.
- Requires relational integration tests for duplicate prevention, eligibility, and send/follow-up outcomes.
