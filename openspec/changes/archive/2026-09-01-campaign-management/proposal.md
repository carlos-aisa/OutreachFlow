## Why

The product is moving from contact-by-contact outreach to periodic campaigns sent to groups of contacts, with results tracked and follow-ups managed over time. Nothing in the system currently represents a campaign — email templates, sender profiles, and drafts exist as loose entities with no container that ties a message, its intended audience, and its outcomes together. `add-campaign-recipient-lifecycle` (an existing, validated change proposal) already depends on this capability existing, but nothing creates it. This change adds that foundation.

## What Changes

- Add a Campaign entity: a name, a short purpose/description, a message (an email template), one or more target audiences (existing contact groups), and an open/closed status.
- Let users create, view, list, and close/reopen campaigns.
- Add a Campaigns workspace screen: a list of campaigns and a detail view showing the campaign's own data (name, status, audience, message) — recipient tracking, send outcomes, and candidate incorporation are explicitly out of scope here and belong to `add-campaign-recipient-lifecycle`, which extends this same detail screen.
- A campaign's audience is defined by referencing existing contact groups, not by duplicating their criteria — reusing `contact-group-management` as already anticipated by the pending recipient-lifecycle change.

## Capabilities

### New Capabilities
- `campaign-management`: Campaign creation, listing, status management, and the base campaign workspace screens.

### Modified Capabilities
(none — this is additive; existing capabilities are referenced, not changed)

## Impact

- Adds a new domain aggregate, EF Core persistence, application/API/OpenAPI contracts, and a Blazor "Campañas" workspace screen following the header/side-panel pattern already used across the app's other list pages.
- Depends on `contact-group-management` (audience selection) and `email-template-management` (message selection); does not modify either.
- Is a prerequisite for `add-campaign-recipient-lifecycle`, which extends the campaign detail screen this change creates with recipient tracking, candidate discovery, and incorporation.
- Requires relational integration tests for campaign persistence and status transitions, and OpenAPI updates for the new endpoints.
