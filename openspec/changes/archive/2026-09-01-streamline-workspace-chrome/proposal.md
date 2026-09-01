## Why

Reviewing the shipped pages surfaced two usability issues: the page header repeats itself three times before any real content appears (a top bar, a small kicker, and the page title all say roughly the same thing), and the sidebar spends a lot of vertical space on a tagline that competes with navigation. Separately, a couple of dropdowns (assigning a contact to a follow-up, filtering by organization) list every record with no search, which will not scale past a handful of entries. There's also a plain untranslated `Common.Create` string showing up on the Contact groups panel button.

## What Changes

- Remove the sticky top bar (`MainLayout`'s `.top-row`) that duplicates the sidebar's workspace branding above every page.
- Remove the small repeated "kicker" line that restates the page title verbatim on every primary page, keeping the title and a rewritten, page-specific description.
- Rewrite each page's description to be short and state that page's actual purpose, replacing generic boilerplate.
- Trim the sidebar header to just the product name, dropping the "Espacio de trabajo" eyebrow and the descriptive paragraph.
- Add `Common.Create`, which was referenced but never defined, so the Contact groups panel's create button shows translated text instead of the raw key.
- Add a reusable search-only combobox (no create option) for picking a single existing contact or organization from a list, and use it for: the Follow-ups task panel's contact picker, the Contacts page's organization filter, and the Draft Generation wizard's organization filter.

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `web-ui-appearance`: trims the page-header requirement (no top bar, no repeated kicker) and the navigation-shell requirement (compact sidebar header), and adds a requirement that single-record contact/organization pickers support search instead of listing every record.

## Impact

- Affected code: `MainLayout.razor`/`.razor.css`, `NavMenu.razor`/`.razor.css`, `app.css`, every primary page's `page-intro` markup and localized description strings, a new shared search-combobox component, `FollowUps.razor`, `Contacts.razor`, `DraftGeneration.razor`, and the `Common.Create` resource key.
- No domain, application, or API contract changes — this is presentation and localization only.
- The `Common.Create` fix and the `.page-kicker` removal apply the same way to every page that already existed with those elements; `Weather.razor` and `Counter.razor` are Blazor scaffold demo pages not reachable from navigation and are left untouched (flagged as separate potential cleanup, not part of this change).
