## Why

Users need understandable, reusable audiences for periodic outreach without repeatedly rebuilding technical filters. Groups must support geographic, organization-type, and tag-based selection while still allowing the user to include or exclude individual contacts.

## What Changes

- Add named contact groups with multiple group memberships per contact.
- Support optional criteria for province, city, organization type, and contact tags.
- Evaluate multiple selected values within a criterion with OR semantics and active criteria with AND semantics.
- Support manual inclusions and exclusions that override criterion-based membership.
- Exclude contacts marked do-not-contact or archived from campaign eligibility, while preserving their group membership history.

## Capabilities

### New Capabilities
- `contact-group-management`: Named contact groups with criteria, calculated membership, and manual membership overrides.

### Modified Capabilities
- `contact-management`: Expose group membership as part of a contact's management experience.

## Impact

- Adds domain, EF Core persistence, application services, API endpoints/OpenAPI contracts, Blazor group and contact screens, and localized text.
- Requires realistic relational integration tests for group evaluation and membership overrides.
- Provides the audience foundation for subsequent campaign proposals.
