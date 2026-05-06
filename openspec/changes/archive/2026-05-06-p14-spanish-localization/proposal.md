## Why

OutreachFlow currently exposes UI and user-facing API messages in a single language, which limits adoption for Spanish-speaking users. Adding Spanish localization improves usability without changing business behavior.

## What Changes

- Add localization infrastructure for the Web application with Spanish translations for navigation, forms, actions, and validation messages.
- Add localized user-facing API error messages in Spanish where applicable.
- Add language selection and default culture handling for Spanish.
- Add tests to validate culture selection and translated output for key workflows.

## Capabilities

### New Capabilities
- spanish-localization: Provide Spanish localized experience for UI and user-facing messages.

### Modified Capabilities
- None.

## Impact

- Affects Web, Api, Application validation messaging, and tests.
- Introduces localization resource files and culture configuration.
- May require OpenAPI examples/notes update for localized error payload expectations.
