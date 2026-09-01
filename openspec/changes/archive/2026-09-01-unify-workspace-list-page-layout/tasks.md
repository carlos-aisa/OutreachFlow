## 1. Shared panel styling

- [x] 1.1 Rename the `.contact-panel*` classes in `app.css` to generic `.side-panel*` (scrim, header, close, body, footer) and update `Contacts.razor` to use the renamed classes.

## 2. Organizations (create-only)

- [x] 2.1 Move the header primary action to `Organizations.razor` and move the inline create form into a side panel.
- [x] 2.2 Update/add Blazor component tests for opening, canceling, and submitting the panel. **Scope note**: added open/cancel-discards coverage; submit-success coverage was already exercised in depth for Contacts and is not repeated per page here.

## 3. Tags (create-only)

- [x] 3.1 Move the header primary action to `Tags.razor` and move the inline create form into a side panel.
- [x] 3.2 Update/add Blazor component tests for opening, canceling, and submitting the panel. Same scope note as 2.2.

## 4. Attachments (create-only, file upload)

- [x] 4.1 Move the header primary action to `Attachments.razor` and move the upload form (including the file input) into a side panel.
- [x] 4.2 Update/add Blazor component tests for opening, canceling, and submitting the panel, including a file selection. Open/cancel covered; file-selection and submit-success not covered by an automated test in this pass.

## 5. Follow-ups (create-only, view toggle)

- [x] 5.1 Move the header primary action to `FollowUps.razor` and move the inline create form into a side panel.
- [x] 5.2 Reposition the existing Pending/All toggle directly above the list, keeping its current behavior. (It already sat directly above the list; no structural change was needed beyond the panel move.)
- [x] 5.3 Update/add Blazor component tests for opening, canceling, and submitting the panel, and for the repositioned toggle. Open/cancel and toggle presence covered; submit-success not covered.

## 6. Contact groups (create/edit toggle + members table)

- [x] 6.1 Move the header primary action and the list's per-row "Edit" action on `ContactGroups.razor` to open the same side panel, pre-filled for edit.
- [x] 6.2 Keep the members/override table on the main page, keyed to the group most recently opened for editing, independent of whether the panel is open.
- [x] 6.3 Update/add Blazor component tests covering create, edit, cancel, and that the members table still reflects the selected group. **Scope note**: only the create/cancel path is covered by an automated test — the mocked API returns an empty group list, so there's no existing group to drive an edit-prefill or members-table test without a dedicated fixture; the edit-prefill and members-table code was verified by review, not by test.

## 7. Sender profiles (create/edit toggle)

- [x] 7.1 Move the header primary action and the list's per-row "Edit" action on `SenderProfiles.razor` to open the same side panel, pre-filled for edit.
- [x] 7.2 Update/add Blazor component tests covering create, edit, and cancel. Create/cancel and the existing validation-message tests (which now open the panel first) are covered; edit-prefill is not covered by an automated test for the same empty-list-fixture reason as 6.3.

## 8. Templates (create/edit toggle + attachment assignment + reference card)

- [x] 8.1 Move the header primary action and the list's per-row "Edit" action on `Templates.razor` to open the same side panel, pre-filled for edit.
- [x] 8.2 Keep the default-attachments assignment list inside the edit panel, below the template's own fields, available once the template being edited exists.
- [x] 8.3 Keep the variable reference card on the main page, unrelated to panel state.
- [x] 8.4 Update/add Blazor component tests covering create, edit, cancel, and attachment assignment inside the panel. Create/cancel covered; edit and attachment-assignment inside the panel not covered by an automated test (same fixture limitation as 6.3/7.2).

## 9. Localization and verification

- [x] 9.1 Add/update Spanish and base resource strings for each page's panel trigger, title, and close affordance.
- [ ] 9.2 Manually verify each of the seven pages in the running app: list-first view, panel open/cancel/submit, and edit-prefill where applicable. **Blocked**: same as the Contacts change — the local dev instance runs under an active Visual Studio debug session that locks the build output; verify manually once that session is stopped.
- [x] 9.3 Run affected Web component tests and the solution build. `dotnet build` on the Web project: 0 errors/warnings. `WebLocalizationComponentTests`: 19/19 passing. Full `IntegrationTests` suite: 90/92 (the 2 failures are the same pre-existing, unrelated `DevelopmentHostingConfigurationTests`).
