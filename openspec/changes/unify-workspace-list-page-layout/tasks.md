## 1. Shared panel styling

- [ ] 1.1 Rename the `.contact-panel*` classes in `app.css` to generic `.side-panel*` (scrim, header, close, body, footer) and update `Contacts.razor` to use the renamed classes.

## 2. Organizations (create-only)

- [ ] 2.1 Move the header primary action to `Organizations.razor` and move the inline create form into a side panel.
- [ ] 2.2 Update/add Blazor component tests for opening, canceling, and submitting the panel.

## 3. Tags (create-only)

- [ ] 3.1 Move the header primary action to `Tags.razor` and move the inline create form into a side panel.
- [ ] 3.2 Update/add Blazor component tests for opening, canceling, and submitting the panel.

## 4. Attachments (create-only, file upload)

- [ ] 4.1 Move the header primary action to `Attachments.razor` and move the upload form (including the file input) into a side panel.
- [ ] 4.2 Update/add Blazor component tests for opening, canceling, and submitting the panel, including a file selection.

## 5. Follow-ups (create-only, view toggle)

- [ ] 5.1 Move the header primary action to `FollowUps.razor` and move the inline create form into a side panel.
- [ ] 5.2 Reposition the existing Pending/All toggle directly above the list, keeping its current behavior.
- [ ] 5.3 Update/add Blazor component tests for opening, canceling, and submitting the panel, and for the repositioned toggle.

## 6. Contact groups (create/edit toggle + members table)

- [ ] 6.1 Move the header primary action and the list's per-row "Edit" action on `ContactGroups.razor` to open the same side panel, pre-filled for edit.
- [ ] 6.2 Keep the members/override table on the main page, keyed to the group most recently opened for editing, independent of whether the panel is open.
- [ ] 6.3 Update/add Blazor component tests covering create, edit, cancel, and that the members table still reflects the selected group.

## 7. Sender profiles (create/edit toggle)

- [ ] 7.1 Move the header primary action and the list's per-row "Edit" action on `SenderProfiles.razor` to open the same side panel, pre-filled for edit.
- [ ] 7.2 Update/add Blazor component tests covering create, edit, and cancel.

## 8. Templates (create/edit toggle + attachment assignment + reference card)

- [ ] 8.1 Move the header primary action and the list's per-row "Edit" action on `Templates.razor` to open the same side panel, pre-filled for edit.
- [ ] 8.2 Keep the default-attachments assignment list inside the edit panel, below the template's own fields, available once the template being edited exists.
- [ ] 8.3 Keep the variable reference card on the main page, unrelated to panel state.
- [ ] 8.4 Update/add Blazor component tests covering create, edit, cancel, and attachment assignment inside the panel.

## 9. Localization and verification

- [ ] 9.1 Add/update Spanish and base resource strings for each page's panel trigger, title, and close affordance.
- [ ] 9.2 Manually verify each of the seven pages in the running app: list-first view, panel open/cancel/submit, and edit-prefill where applicable.
- [ ] 9.3 Run affected Web component tests and the solution build.
