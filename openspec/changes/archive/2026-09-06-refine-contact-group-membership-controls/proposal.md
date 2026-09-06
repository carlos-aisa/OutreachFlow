## Why

The redesigned contact group membership view shipped without list-level filtering or visibility into group size, allowed creating groups with no matching criteria (making them silent catch-all groups), and had two membership-view bugs: the member list did not default to showing current members, and manually excluded contacts leaked into both the "members" and "non-members" filtered views. The detail page's summary badge also stayed fixed to the total member count instead of the currently filtered count, breaking the app-wide convention that list badges reflect the visible result set.

## What Changes

- Add a name filter and a per-group contact-count badge to the contact groups list page.
- **BREAKING**: Require at least one membership criterion (province, city, organization type, or tag) when creating or updating a contact group; reject empty-criteria requests with a validation error.
- Default the contact group detail page's status filter to "Members" on load.
- Fix membership status classification so manually excluded contacts are correctly excluded from the "Members" filter and correctly included in the "Non-members" filter (previously they were invisible in both).
- Fix the detail page's contact-count summary badge to reflect the currently filtered/visible contacts instead of a fixed total, consistent with other list badges in the app.

## Capabilities

### Modified Capabilities
- `contact-group-management`: Criteria are now required (at least one) instead of optional; a manual-only, no-criteria group can no longer be created or updated.

## Impact

- Affected code: `ContactGroupService` (Application layer validation), `ContactGroups.razor` (list page), `ContactGroupDetail.razor` (detail/member page), the fake-data seeder (removed a no-criteria seed group), and related resx localization keys.
- Existing integration tests that relied on empty-criteria groups as campaign audience scaffolding were updated to use a placeholder criterion or an explicit manual-include override.
- No database schema changes; no API route changes (validation moves the empty-criteria case from 201 to 400).
