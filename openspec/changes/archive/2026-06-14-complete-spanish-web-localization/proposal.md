## Why

OutreachFlow already supports Spanish culture selection and some localized Web surfaces, but large parts of the Web UI still render English labels, placeholders, buttons, and page copy. Now that language selection works reliably, the remaining untranslated screens are more visible and create an inconsistent Spanish experience.

## What Changes

- Complete Spanish localization coverage for Web pages and workflows that still contain user-facing English text.
- Add missing `SharedResource` keys and Spanish translations for affected labels, placeholders, headings, actions, and empty-state copy.
- Update Razor components that still use hardcoded English strings so they resolve localized text through the existing localization pattern.
- Extend automated Web localization coverage to verify representative Spanish rendering across the newly localized workflows.
- Update localization documentation if current docs overstate or underspecify which Web workflows are fully localized.

## Capabilities

### New Capabilities
<!-- None. -->

### Modified Capabilities
- `spanish-localization`: extend the requirement coverage from selected core workflows to the remaining user-facing Web surfaces that still render English text under Spanish culture.

## Impact

- Affects `OutreachFlow.Web` Razor pages, shared resource files, and localized UI rendering tests.
- Likely updates `src/OutreachFlow.Web/Resources/SharedResource*.resx` plus multiple page components under `src/OutreachFlow.Web/Components/Pages/`.
- May require documentation updates in `docs/localization/LOCALIZATION.md` and user-facing project docs if localization scope changes become explicit.
