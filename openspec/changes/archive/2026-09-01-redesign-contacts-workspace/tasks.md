## 1. Page layout

- [x] 1.1 Reorder `Contacts.razor` so the page header carries the primary "New contact" action, the filter bar sits directly above the results, and the contact list renders by default.
- [x] 1.2 Remove the permanent inline contact creation section from the page body.

## 2. Contact creation panel

- [x] 2.1 Add a dismissible side panel component/markup triggered by the header's "New contact" action, containing the existing contact fields (name, email, phone, role, source, status, do-not-contact).
- [x] 2.2 Wire panel Cancel and dismiss (backdrop/close) actions to discard entered data without submitting.
- [x] 2.3 Wire panel submit to the existing `ContactApiClient.CreateIntakeAsync` call, close the panel on success, and keep the existing contextual next-action confirmation.

## 3. Organization search-or-create combobox

- [x] 3.1 Replace the organization `<InputSelect>` and "create organization" checkbox with a single combobox bound to the already-loaded organizations list, filtering client-side as the user types.
- [x] 3.2 Show a "Create organization "<text>"" option when no existing organization matches the typed text, and selecting it marks the field as a pending new organization.
- [x] 3.3 Reveal the optional extra organization fields (type, website, city, province, country) behind an explicit "add more details" action only after a new organization is chosen.
- [x] 3.4 Update the submit handler to send `OrganizationId` for an existing selection or `NewOrganization` for a pending new organization, matching the current `CreateContactIntakeRequest` contract.

## 4. Localization and tests

- [x] 4.1 Add/update Spanish and base resource strings for the new panel, combobox, and "create organization" option.
- [x] 4.2 Add or update Blazor component tests covering: opening/canceling the panel, selecting an existing organization, creating a new organization inline, and the panel-closed contact list as the default view.

## 5. Verification and documentation

- [ ] 5.1 Manually verify the flow in the running app: list-first view, panel open/cancel/submit, existing-organization selection, new-organization creation with and without extra details. **Blocked**: the local dev instance was running under an active Visual Studio debug session that locks the build output; verify manually once that session is stopped (or via hot reload) before merging.
- [x] 5.2 Run affected Web component tests and the solution build.
