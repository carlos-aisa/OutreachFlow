## Why

The Contacts page redesign established a shared list-page layout (primary action in the header, filters directly above the list, record creation in a dismissible side panel), but the app's other list pages still use the old stacked layout with a permanent inline form. `web-ui-appearance` already commits the app to feeling like "one product instead of isolated Bootstrap defaults" — today only Contacts satisfies that for this pattern.

## What Changes

- Apply the shared layout (header primary action, list as the default view, side panel for record creation) to `Organizations.razor`, `Tags.razor`, `Attachments.razor`, and `FollowUps.razor`, whose forms are create-only today.
- Apply the same side panel to `ContactGroups.razor`, `SenderProfiles.razor`, and `Templates.razor`, which currently swap one inline form between "create" and "edit" mode; both the header's "New" action and a row's "Edit" action will open the same panel, pre-filled for edit.
- Keep page-scoped secondary content that isn't part of a single record's own fields on the main page rather than in the panel: `ContactGroups`' member list and manual overrides stay on the page for whichever group is selected; `Templates`' variable reference card stays on the page. `Templates`' default-attachments assignment stays inside the edit panel, since it's part of editing that one template.
- Reposition `FollowUps`' existing Pending/All toggle directly above the list, consistent with how filters sit relative to results elsewhere; it stays a view toggle, not a new filter form.
- Generalize the side panel CSS added for Contacts (`.contact-panel*`) into shared `.side-panel*` classes used by all eight pages, so there is one implementation to maintain instead of eight copies.

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `web-ui-appearance`: extends the record-creation side panel requirement (added for Contacts) with scenarios confirming the pattern on the other seven pages, including the create/edit unification for pages that previously toggled one inline form.

## Impact

- Affected code: `Organizations.razor`, `Tags.razor`, `ContactGroups.razor`, `SenderProfiles.razor`, `Templates.razor`, `Attachments.razor`, `FollowUps.razor`, and `app.css` (renaming/generalizing the panel classes introduced for Contacts).
- No domain, application, or API contract changes — every affected page already has the data operations it needs (create, update/edit, list) through its existing API client.
- Requires updated Blazor component tests and localization strings per page, per the project's quality bar for behavior changes.
- Out of scope: Dashboard, Settings, Drafts, and Imports pages — they weren't identified as matching this filters/create/list structure during the original review.
