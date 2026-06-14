## Why

OutreachFlow exposes a language selector in the Web sidebar, but changing the selection does not actually switch the UI away from English or persist the user's preference across application restarts. The selector is also visually misaligned in the sidebar, so the current experience suggests localization is supported while the primary language-selection flow is unreliable.

## What Changes

- Fix the Web language-selection flow so selecting English or Spanish applies the chosen culture to localized UI surfaces immediately after the redirect.
- Persist the user's selected Web culture so the application restores the last chosen language on later visits and after restarting the application.
- Correct the sidebar placement of the language selector so it fits the existing layout on supported viewport sizes.
- Add focused test coverage for culture selection, persistence, and at least one localized Web surface that proves the selected culture is being applied.

## Capabilities

### New Capabilities
- None.

### Modified Capabilities
- `spanish-localization`: Strengthen culture selection requirements so the Web language selector applies the chosen culture, persists the user's preference, and presents the selector correctly in the sidebar layout.

## Impact

- Affects `OutreachFlow.Web` localization and sidebar UI behavior.
- May update request-culture cookie handling, localized Web component rendering, and sidebar styles/layout.
- Requires Web integration or component tests for language switching and persistence, plus localization documentation updates if behavior becomes more explicit.
