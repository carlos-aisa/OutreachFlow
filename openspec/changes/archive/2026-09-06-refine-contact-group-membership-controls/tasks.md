## 1. Contact groups list page

- [x] 1.1 Add a name filter (search input + clear action) to the contact groups list page.
- [x] 1.2 Load and display a per-group contact-count badge, with the list's own count badge reflecting the filtered result set.
- [x] 1.3 Add a dedicated empty-state message for a filtered, empty groups list.

## 2. Mandatory group criteria

- [x] 2.1 Reject `CreateAsync`/`UpdateAsync` requests with an empty criteria list in `ContactGroupService` with a validation error.
- [x] 2.2 Add a client-side guard in the contact group form so submitting with no criteria shows an inline error instead of calling the API.
- [x] 2.3 Update the fake-data seeder to stop creating a no-criteria "catch-all" group.
- [x] 2.4 Update existing integration/unit tests that relied on empty-criteria scaffolding groups (placeholder criterion or explicit manual-include override where real criteria matching was required).
- [x] 2.5 Add unit tests covering rejection of empty criteria on create and on update.

## 3. Contact group membership view fixes

- [x] 3.1 Default the detail page's status filter to "Members" on load and on Clear.
- [x] 3.2 Fix membership classification so "Members" excludes manually-excluded contacts and "Non-members" includes them.
- [x] 3.3 Bind the contact-count summary badge to the currently filtered contact list instead of a fixed member count.

## 4. Verification

- [x] 4.1 Add/adjust component tests for list filtering, per-group badges, the criteria-required guard, default member filter, member/non-member classification, and the filter-aware summary badge.
- [x] 4.2 Run the full solution test suite and confirm no regressions.
