## Why

The Contacts page currently stacks search filters, an always-visible contact creation form, and the results list with no priority order, so users scroll past a full form to reach data they already have. The "create a new organization" path is also a hidden checkbox even though most new contacts introduce a new organization, not an existing one.

## What Changes

- Reorder the Contacts page so the page header carries the primary "New contact" action, a compact filter bar sits directly above the results, and the contact list is the default, dominant content.
- Move contact creation out of a permanent inline section into a side panel opened by the primary action, with explicit Create/Cancel controls, so the list stays the page's default view.
- Replace the "create a new organization" checkbox and separate organization dropdown with a single search-or-create combobox: typing filters existing organizations, and an explicit "Create organization "<name>"" option appears when there is no match. Picking it marks the field as a new organization and reveals the optional extra organization fields (type, website, city, province, country) only when the user asks for them.
- Establish this list-page layout (primary action in the header, filters directly above the list, record creation in a side panel) as the shared convention in the web appearance system, for later reuse on structurally similar list pages. Only the Contacts page is updated in this change.

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `contact-management`: the contact management UI requirement changes — contact creation moves from a permanent inline form into a side panel, and organization association uses a search-or-create combobox instead of a checkbox-revealed subform.
- `web-ui-appearance`: adds a requirement establishing the record-creation side panel as the shared pattern for primary list pages, and updates the progressive-disclosure scenario for contact intake to describe the search-or-create combobox instead of a checkbox toggle.

## Impact

- Affected code: `src/OutreachFlow.Web/Components/Pages/Contacts.razor` and its use of `OrganizationApiClient`/`ContactApiClient`.
- No domain, application, or API contract changes: `CreateContactIntakeRequest` and `CreateOrganizationRequest` already carry the fields this UI needs (organization name required, all other organization fields optional), so no EF Core migration or OpenAPI update is required.
- Requires updated Blazor component tests and localization strings for the new side panel and combobox interactions, per the project's quality bar for behavior changes.
- Establishes a layout pattern intended for later reuse on other list pages (organizations, tags, contact groups, sender profiles, templates, attachments, follow-ups); rolling it out to those pages is explicitly out of scope here.
