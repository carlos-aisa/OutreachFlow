## 1. Layout chrome

- [x] 1.1 Remove `MainLayout.razor`'s `.top-row` bar and its now-unused CSS in `MainLayout.razor.css`.
- [x] 1.2 Trim `NavMenu.razor`'s sidebar header to just the product name; remove the eyebrow and description paragraph and their now-unused CSS in `NavMenu.razor.css`, keeping the header compact.

## 2. Page headers

- [x] 2.1 Remove the `.page-kicker` line from every primary page's `page-intro` block (Contacts, Organizations, Tags, ContactGroups, SenderProfiles, Templates, Attachments, FollowUps, Home, Drafts, DraftGeneration, DraftDetail, Imports, ContactDetail, Settings, Error).
- [x] 2.2 Rewrite each of those pages' description resource strings (base + Spanish) to a short, page-specific sentence stating that page's actual purpose.

## 3. Common.Create fix

- [x] 3.1 Add the missing `Common.Create` resource key (base + Spanish) so the Contact groups panel's create button renders translated text.

## 4. Search-only picker component

- [x] 4.1 Add a shared, generic `SearchSelect` Blazor component (search-as-you-type, select from existing items, no create option) reusing the `.combo-wrap`/`.combo-dropdown`/`.combo-item` styles already shipped for Contacts. Also added a clear (×) affordance so filter usages can reset to "any" without retyping.
- [x] 4.2 Replace `FollowUps.razor`'s contact `InputSelect` with the shared component.
- [x] 4.3 Replace `Contacts.razor`'s organization filter `<select>` with the shared component.
- [x] 4.4 Replace `DraftGeneration.razor`'s organization filter `<select>` with the shared component.

## 5. Verification

- [x] 5.1 Update/add Blazor component tests. **Scope note**: covered — sidebar header compact (fixed existing test to target `.navbar-brand`), Contact groups create button shows translated "Crear" (new assertion), and one full search/select/verify pass through `SearchSelect` on the Follow-ups contact picker (new test, with a dedicated single-contact HTTP fixture). Not covered by a dedicated test: a page-by-page "no kicker" assertion (kicker removal was verified by review across all 16 files plus the existing per-page localization tests still passing), and the Contacts/DraftGeneration organization-filter `SearchSelect` instances specifically (same component, already exercised by the Follow-ups test and by code review).
- [ ] 5.2 Manually verify in the running app. **Blocked**: same recurring blocker — the local dev instance runs under an active Visual Studio debug session that locks the build output.
- [x] 5.3 Run affected Web component tests and the solution build. `dotnet build` on the Web project: 0 errors/warnings throughout. `WebLocalizationComponentTests`: 20/20 passing. Full `IntegrationTests` suite: 91/93 (the 2 failures are the same pre-existing, unrelated `DevelopmentHostingConfigurationTests`).
