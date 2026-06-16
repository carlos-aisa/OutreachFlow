## Why

OutreachFlow currently exposes language selection from the sidebar header and has no dedicated place for user preferences, while theme selection does not exist yet. A proper settings page gives the application a stable home for web preferences and avoids overloading the navigation shell with controls that belong in configuration.

## What Changes

- Add a dedicated web settings page reachable from the main navigation.
- Move the language selector from the sidebar header into a general preferences section on the settings page.
- Add a theme selector on the settings page for light, dark, and system behavior.
- Persist both language and theme preferences across navigation, reloads, and later visits.
- Adapt the shared visual system so the settings page and the rest of the workspace remain coherent in light and dark themes.

## Capabilities

### New Capabilities
- `web-settings-preferences`: A dedicated settings page for general web preferences, including language and theme selection.

### Modified Capabilities
- `web-ui-appearance`: Update the navigation shell and shared workspace appearance to include a settings destination and remove preference controls from the sidebar header.
- `spanish-localization`: Localize the settings page, including language and theme preference labels and options, when Spanish is active.

## Impact

- Affected code in Blazor layout/navigation components, settings page components, web static assets, CSS design tokens, and client-side preference persistence.
- No API contract changes are expected.
- Requires UI or integration coverage for settings navigation, language persistence, theme persistence, and cross-page behavior.
- Requires localized resource updates for the new settings surfaces in English and Spanish.
