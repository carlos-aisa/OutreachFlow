## Context

OutreachFlow already declares `en-US` and `es-ES` as supported Web cultures and exposes a sidebar selector that redirects to `/culture/set`. The intended flow is straightforward: select a culture, write the culture cookie, redirect back, and let localized components render using the new request culture. In practice, the selected language is not taking effect, the preference is not retained across later visits, and the selector is visually crowding the sidebar header.

This change is narrower than full Spanish coverage. It focuses on making the language-selection mechanism reliable for the parts of the Web UI that already use `IStringLocalizer`, while also correcting the selector's placement inside the existing sidebar layout.

## Goals / Non-Goals

**Goals:**
- Ensure selecting a supported language changes the culture used by localized Web components on the next render.
- Persist the selected Web culture so later visits keep the user's preference.
- Correct the sidebar layout so the selector does not overlap or push into the navigation list.
- Add tests that prove the selector flow changes and restores culture as expected.

**Non-Goals:**
- Localize every currently hardcoded English page in the Web app.
- Add support for languages beyond English and Spanish.
- Change the API localization contract beyond what is needed to keep culture behavior consistent.
- Redesign the full sidebar visual system beyond placing the selector correctly.

## Decisions

- Persist the culture preference with a root-scoped Web cookie.
  - Rationale: the current flow already relies on `CookieRequestCultureProvider`, so the least invasive fix is to ensure the stored cookie is visible to the whole site and survives later visits.
  - Alternative considered: move persistence to query-string-only culture. Rejected because query strings are not durable user preference storage and would keep polluting navigation URLs.

- Keep the current culture-provider priority and fix the selector flow rather than replacing the localization stack.
  - Rationale: explicit selection first, then cookie, then `Accept-Language`, already matches the desired behavior. The issue appears to be in preference persistence and application, not in the priority model itself.
  - Alternative considered: reorder providers or remove the query-string provider. Rejected because it does not directly address the reported failure.

- Verify the behavior using a Web-facing test that exercises the selector persistence path and a component-level localized rendering check.
  - Rationale: current tests prove localized rendering under a forced culture, but not the end-to-end selector behavior. We need at least one test of the actual selection/persistence flow and one localized UI assertion to avoid false confidence.
  - Alternative considered: rely only on manual browser verification. Rejected because the bug is easy to regress.

- Reposition the selector within a dedicated sidebar header area instead of letting it compete with the existing fixed-height top row.
  - Rationale: the current header height is tuned for a single-row title area. Giving the selector an explicit slot avoids overlap and makes the menu start consistently below it.
  - Alternative considered: shrink the selector or reduce spacing inside the current row. Rejected because it is brittle across viewport sizes and does not address the structural layout issue.

## Risks / Trade-offs

- [Cookie persistence fix still appears broken in certain local launch profiles] -> Mitigation: verify behavior under the actual Web host profiles used in development and cover the root cookie behavior in tests.
- [Localized pages still appear mostly English after the fix] -> Mitigation: scope this change explicitly to selection/persistence/layout and follow with a separate localization-coverage change.
- [Sidebar layout fix changes spacing on desktop or mobile] -> Mitigation: keep the visual change local to the sidebar header and verify both narrow and wide breakpoints.
- [Tests become too coupled to markup details] -> Mitigation: assert visible localized text and culture behavior rather than brittle full-markup snapshots.

## Migration Plan

1. Reproduce the selector behavior and confirm which localized surfaces fail to change after selection.
2. Update the selector persistence mechanism so the selected culture is available on subsequent requests across the site.
3. Adjust the sidebar header layout to give the selector a stable placement.
4. Add targeted tests for persisted culture selection and visible localized rendering.
5. Update localization guidance if the persistence behavior or selector expectations need to be documented more explicitly.

Rollback strategy: revert the selector persistence and layout changes together, returning to the previous default-culture behavior while preserving unrelated localization work.

## Open Questions

- Do we want the sidebar selector to remain in the header row or move into a stacked header block beneath the app title?
- Should we also localize the selector option labels themselves as part of this change, or leave them as `English` and `Español`?
