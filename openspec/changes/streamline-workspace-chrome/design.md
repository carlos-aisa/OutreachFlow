## Context

Every primary page currently renders three header layers before real content: `MainLayout`'s sticky `.top-row` bar (workspace-level kicker + caption), then each page's own `.page-intro` block, which itself repeats the page title as a small `.page-kicker` line directly above the `<h1>` of the same title. The sidebar's brand panel additionally carries a "Espacio de trabajo" eyebrow and a multi-line description. All of this was reviewed against a screenshot from the shipped Contacts page. Separately, `FollowUps.razor`'s contact picker, `Contacts.razor`'s organization filter, and `DraftGeneration.razor`'s organization filter are plain `<select>`/`InputSelect` elements bound to the full unfiltered contact/organization list.

## Goals / Non-Goals

**Goals:**
- Cut the header down to one title + one purpose-specific description per page, with no separate top bar and no repeated kicker line.
- Keep the sidebar header to just the product name.
- Let a user find one contact or organization by typing, in the three places identified as plain full-list dropdowns.
- Fix the `Common.Create` untranslated string.

**Non-Goals:**
- No rework of the sidebar navigation items themselves, icons, or active-state styling.
- No change to `Weather.razor`/`Counter.razor` (unreferenced Blazor scaffold pages) — flagged to the user as a separate possible cleanup, not touched here.
- No "search or create" behavior for these three pickers — unlike the Contacts page's organization field, these only ever pick an *existing* record (assigning a follow-up to a contact, or filtering by an existing organization), so there is no create path to support.
- No change to `Settings.razor`'s `Settings.Apply.Kicker` — that is a small card's own eyebrow label next to its own heading, not a page-level kicker repeating the page's `<h1>`.

## Decisions

**Extract a shared search-combobox component now, not per page.** The Contacts page's organization combobox (search-or-create) was kept page-local because it had one consumer. It now has three consumers for the search-only variant (Follow-ups, Contacts filter, Draft Generation filter), which is exactly the trigger the earlier design doc named for extracting a shared component. Build `Components/Shared/SearchSelect.razor` as a generic Blazor component (`@typeparam TItem`) taking the item list, a display-text selector, and a two-way-bound selected id; it renders the same `.combo-wrap`/`.combo-dropdown`/`.combo-item` markup and CSS already shipped for Contacts, without a "create new" row. The Contacts page's existing organization *create* combobox stays as its own page-local implementation (it still needs the create-row behavior this component doesn't have); only the three plain-`<select>` pickers move to the new shared component.

**Drop the top bar rather than repurpose it.** The sidebar already establishes "OutreachFlow workspace" branding; the top bar said the same thing again in different words. Removing it outright (rather than giving it new content) is simpler and matches the ask to reduce vertical chrome, not relocate it.

**Rewrite descriptions per page, not with a template.** A generic replacement sentence would just be new boilerplate. Each page's description gets rewritten from what that page actually does, using its existing UI (filters, panel fields, list columns) as the source of truth rather than guessing.

**Sidebar keeps a thin brand line, not zero header.** Removing the eyebrow and description text but keeping "OutreachFlow" as a simple, compact header (no bordered/gradient panel box) preserves orientation ("what app is this") while minimizing vertical space, per the answered design question to leave the product name untranslated and unembellished.

## Risks / Trade-offs

- Removing the top bar and kicker reduces redundancy but also removes a location where future global-workspace messaging (e.g., an environment banner) could have gone → Mitigation: not a current need; revisit if one arises rather than keeping unused chrome around speculatively.
- Rewriting 15+ description strings by hand risks inconsistent tone across pages → Mitigation: keep each description to one short sentence stating the page's primary action and scope, mirroring the phrasing style already used in the better existing descriptions (e.g., Contacts', FollowUps').
- A new shared `SearchSelect` component adds one more piece of shared UI surface to maintain → Accepted; it now has three real consumers, which is exactly the threshold the earlier design set for extraction.

## Open Questions

(none — the two open questions from the original ask, product name handling and rollout scope, were settled with the user before this change was written.)
