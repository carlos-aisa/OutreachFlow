## Context

OutreachFlow already has a working Spanish culture-selection mechanism, shared Web resource files, and partial localization coverage for navigation plus selected management workflows. After fixing language persistence, the remaining untranslated areas are more obvious because users can now switch to Spanish reliably and keep that preference while navigating.

The current gap is mostly presentation-level: several Razor pages still contain hardcoded English titles, labels, placeholders, button text, loading states, and empty-state copy. Representative examples appear in `Home.razor`, `DraftGeneration.razor`, `DraftDetail.razor`, `FollowUps.razor`, `Drafts.razor`, `Attachments.razor`, and `MainLayout.razor`. The project already standardizes on `IStringLocalizer<SharedResource>` plus `SharedResource.resx` / `SharedResource.es.resx`, so the change should extend that pattern rather than introducing a second localization mechanism.

## Goals / Non-Goals

**Goals:**
- Complete Spanish localization for the remaining user-facing Web screens that still render English text under `es-ES`.
- Add the missing resource keys and translations needed for headings, placeholders, table headers, button labels, loading text, and empty states in affected pages.
- Keep localization changes consistent with the existing shared-resource approach so future screens follow one pattern.
- Add or extend Web localization tests to cover representative pages beyond the navigation and contacts surfaces already exercised.

**Non-Goals:**
- Redesign page layouts, navigation structure, or visual styling beyond any minimal text-fit adjustments strictly required by translation length.
- Add support for languages beyond English and Spanish.
- Rework the culture-selection mechanism, persistence strategy, or localization-provider order that was already fixed in the previous change.
- Localize internal-only diagnostics, developer-facing logs, or non-user-facing technical strings unless they appear directly in the Web UI.

## Decisions

- Use `IStringLocalizer<SharedResource>` for all newly localized Web strings.
  - Rationale: this is already the active pattern in localized pages such as `Contacts`, `Organizations`, `Tags`, `Templates`, and `NavMenu`. Extending the same resource source keeps translation lookup centralized and avoids fragmented per-page resource files.
  - Alternative considered: introduce per-page `.resx` files for each Razor page. Rejected because the current repo conventions and existing coverage already rely on `SharedResource`, and a mixed model would add overhead without clear benefit for this scope.

- Localize the change in workflow-oriented batches rather than string-by-string across the whole repo.
  - Rationale: pages such as `Home`, `DraftGeneration`, `DraftDetail`, `Drafts`, `FollowUps`, `Attachments`, and residual layout copy each expose clusters of related strings. Updating them as page-level batches makes it easier to reason about completeness, test intent, and documentation coverage.
  - Alternative considered: only translate obviously visible headings and leave secondary states for later. Rejected because partial translation inside one page produces the same inconsistent Spanish experience this change is meant to remove.

- Prefer additive resource-key expansion over renaming existing keys unless duplication is clearly harmful.
  - Rationale: the shared resource file already contains a mix of `Nav.*`, `Common.*`, and feature-specific keys. Adding missing feature keys is lower risk than renaming existing ones and avoids churn in already working localized pages.
  - Alternative considered: normalize the entire resource catalog as part of this change. Rejected because it would broaden scope into refactoring rather than completing localization coverage.

- Verify coverage with representative component tests rather than attempting exhaustive snapshot coverage of every localized page.
  - Rationale: localized rendering tests should prove that the change reaches newly covered workflows, but full-page snapshot assertions would be brittle and expensive to maintain. A focused set of representative assertions across the newly localized pages gives strong regression coverage with lower maintenance cost.
  - Alternative considered: rely only on manual checks because this is “just copy.” Rejected because localization regressions are easy to reintroduce when future UI changes add new hardcoded strings.

## Risks / Trade-offs

- [Some pages still contain hidden or less obvious English strings after the first pass] -> Mitigation: inventory user-facing strings by workflow and add representative tests for each batch of newly localized pages.
- [Shared resource files become harder to navigate as more keys are added] -> Mitigation: keep new keys grouped by feature prefix and avoid unnecessary renames during this change.
- [Spanish translations increase text length and expose layout wrapping issues] -> Mitigation: keep any visual adjustments minimal and limited to components where translation length causes obvious clipping or overlap.
- [Tests become too coupled to exact phrasing] -> Mitigation: assert a few high-signal translated labels per page rather than entire markup snapshots.

## Migration Plan

1. Identify the remaining Web pages and layout fragments that still render English strings under Spanish culture.
2. Add the required English baseline and Spanish translation keys to `SharedResource.resx` and `SharedResource.es.resx`.
3. Replace hardcoded user-facing strings in the affected Razor pages with localized lookups.
4. Extend Web localization tests to cover representative newly localized workflows.
5. Update localization documentation if the supported Spanish coverage description becomes more explicit after implementation.

Rollback strategy: revert the resource-file additions, Razor localization substitutions, and related tests together to return the affected pages to their previous mixed-language state without affecting the already shipped language-selection mechanism.

## Open Questions

- Should the `About` link in `MainLayout` remain as-is because it points to external Microsoft documentation, or should the label itself be localized even though the destination content stays English?
- Do we want this change to include the scaffold/sample `Weather` page if it is still reachable in the Web app, or should sample/demo pages be treated as out of scope unless they are part of the supported product surface?
