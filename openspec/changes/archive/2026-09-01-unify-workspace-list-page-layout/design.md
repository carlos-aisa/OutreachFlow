## Context

Seven pages share a rough shape (an inline form plus a results list) but differ in real ways: `Organizations`, `Tags`, `Attachments`, and `FollowUps` have create-only forms with no edit path; `ContactGroups`, `SenderProfiles`, and `Templates` reuse one inline form for both create and edit, switching on a local flag (`selectedGroup is null`, `editingId is null`, `editingId is null` respectively — three different names for the same idea). `ContactGroups` also shows a members/override table only when a group is selected, and `Templates` shows a default-attachments assignment list only when editing, plus an always-visible variable reference card. This survey was done by reading all seven files in full before drafting this design.

## Goals / Non-Goals

**Goals:**
- Give all eight list pages (Contacts included) the same page shape: header primary action, list as the default view, side panel for record creation and editing.
- Unify create and edit into one panel for the three pages that currently toggle an inline form, instead of leaving edit inline while only create moves to a panel.
- Reuse one set of panel CSS classes across all pages instead of duplicating Contacts' page-specific styles.

**Non-Goals:**
- No new "search or create" combobox — the survey found no pick-existing-or-create-new field on any of these seven pages (the closest look-alikes are plain existing-only pickers or free-text duplicate fields, neither of which has the majority-creates-new pattern that motivated the Contacts combobox).
- No change to page-scoped secondary content's own behavior: `ContactGroups` member/override management and `Templates`' variable reference card keep their current logic, only their position relative to the new panel changes where relevant.
- No change to `Attachments`' file upload mechanics beyond moving its container into the panel.
- Dashboard, Settings, Drafts, and Imports are not touched.

## Decisions

**Edit moves into the panel too, not just create.** For `ContactGroups`, `SenderProfiles`, and `Templates`, both the header's "New" action and a list row's "Edit" action open the same side panel, pre-filled when editing. Alternative considered: leave the existing inline edit-in-place behavior and only move the create path into a panel. Rejected — that would leave two different interaction patterns on the same page (panel for new, inline swap for edit), which is more inconsistent than today's single inline form.

**Page-scoped secondary content stays on the page, keyed to the record being edited.** `ContactGroups`' member list and override controls need a selected group's id and operate on a set of related records (contacts), not the group's own fields — they stay on the main page and key off whichever group is currently open in the panel (or was most recently edited). `Templates`' variable reference card is independent of any specific template and stays on the page. `Templates`' default-attachments assignment, by contrast, is part of editing one template's own data, so it stays inside the edit panel below the template's fields, the same place it renders today.

**`FollowUps`' Pending/All toggle stays a view toggle, not a new filter form.** It already sits in the list's header; this change only repositions it to sit directly above the list content, matching how filters relate to results elsewhere, without turning it into a full filter bar it was never meant to be.

**Generalize the panel CSS.** The `.contact-panel*` classes added for Contacts get renamed to `.side-panel*` (scrim, header, close, body, footer) in `app.css`, and `Contacts.razor` is updated to the new class names alongside the other seven pages. One implementation, reused everywhere, instead of near-duplicate CSS per page.

## Risks / Trade-offs

- Unifying create/edit into one panel changes a familiar inline-edit interaction on three pages at once → Mitigation: the resulting panel behavior (open pre-filled, Cancel discards, Save/Create persists) is identical in shape to what Contacts already ships and was reviewed; per-page component tests will cover both open-for-create and open-for-edit paths.
- `ContactGroups`' members table depends on "which group is currently relevant" once editing moves into a panel and can be closed → Mitigation: keep the last-selected/edited group's id in page state independent of panel-open state, so closing the panel doesn't hide the members table if the user was mid-review of that group's membership.
- Renaming shared CSS classes touches `Contacts.razor` again in the same change that touches seven other pages → Mitigation: the rename is mechanical (class name only, no behavior change) and covered by re-running the existing Contacts component tests.

## Open Questions

- Should `Templates`' attachment-assignment list require the template to already exist (as today, `editingId is not null`), meaning it's unavailable while creating a brand-new template in the panel until the first save? Keeping today's behavior is the default assumption unless the user wants attachment assignment available immediately on create.
