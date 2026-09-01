## Context

The Contacts page (`Contacts.razor`) currently renders three stacked cards in a fixed order: filters, an always-visible contact creation form, then the results table. The creation form includes a checkbox that reveals a nested "new organization" sub-form. This was reviewed against a working interactive mockup (built and validated with the user before this change) that reorders the page and replaces the checkbox-driven organization sub-form with a single search-or-create combobox. Product input during that review: roughly 90% of new contacts also introduce a new organization, so the common path should not require an extra toggle click.

## Goals / Non-Goals

**Goals:**
- Make the contact list the default, dominant view of the page; move creation into an on-demand side panel.
- Let a user resolve an organization — existing or brand new — through one field instead of a select plus a checkbox-revealed sub-form.
- Keep the change confined to presentation: no domain, application, or API contract changes.
- Establish a page-layout convention (`web-ui-appearance`) other structurally similar list pages can adopt later.

**Non-Goals:**
- Rolling the layout pattern out to other list pages (organizations, tags, contact groups, sender profiles, templates, attachments, follow-ups) — tracked as future work.
- Applying the same search-or-create combobox pattern to tag assignment on the Contact detail page — a second known use case, deferred.
- Extracting a shared, reusable Blazor component for the side panel or the combobox — this change has one consumer; extraction is deferred until a second page needs the pattern.
- Any change to how organizations or contacts are validated, persisted, or exposed via the API.

## Decisions

**Side panel instead of a centered modal.** The create form's height varies (it grows when the optional extra organization fields are shown), and a centered modal forces internal scrolling inside a small box. A right-anchored panel gives more usable vertical space. Bootstrap's modal CSS/JS is already bundled and unused elsewhere in the app, so it was a real alternative, but was rejected for this specific form shape. If a future page's create form is short and fixed-height, a centered modal remains reasonable for that page — this decision is not meant to force one pattern for every future case.

**Single search-or-create combobox instead of select + checkbox.** Today's UI defaults to the minority case (attach to an existing organization) and requires an extra click to reach the majority case (create one). A combobox that offers "Create organization "<text>"" when there is no match makes the common path the fastest one, without adding a control for the 10% who do pick an existing organization — they type and select exactly as before.

**Client-side filtering over a new search endpoint.** The page already calls `OrganizationApiClient.ListAsync()` once on load to populate the existing filter dropdown. The combobox reuses that same in-memory list and filters it client-side rather than adding a debounced server search. This avoids a new endpoint for a UI-only change. It assumes the organization list stays small enough to load in full, which matches current data volume.

**Page-local implementation, not a shared component yet.** The panel and combobox are implemented directly in `Contacts.razor` rather than extracted into shared components. A second consumer (tag assignment on `ContactDetail.razor`) is already known, but extracting now would be designing for a requirement this change doesn't need yet. Revisit extraction when that second page is actually built.

## Risks / Trade-offs

- Moving creation off the page into a panel could make it less discoverable → Mitigation: the "New contact" button sits in the page header's primary-action position, which is exactly where `web-ui-appearance`'s page-header requirement expects a primary action to be.
- Client-side organization filtering degrades if the organization list grows large → Mitigation: acceptable at current data volume; revisit with a server-side search endpoint if organization counts grow substantially (left as an open question below, not solved here).
- A dropdown that closes on input blur can race with a click on one of its options → Mitigation: close on blur with a short delay so a pending click still registers; verify keyboard interaction (Escape to close, arrow keys / Enter to select) during implementation.
- Building the panel and combobox page-local rather than shared means some duplicated markup if a second page adopts the pattern soon → Accepted deliberately; revisit when that happens rather than generalizing now.

## Open Questions

- Should the search-or-create combobox be extracted into a shared component now, given tag assignment on `ContactDetail.razor` is a known second use case, or only once that page is actually changed?
- Should the optional extra organization fields (type, website, city, province, country) gain any validation when created inline, or stay exactly as permissive as the existing organization form?
