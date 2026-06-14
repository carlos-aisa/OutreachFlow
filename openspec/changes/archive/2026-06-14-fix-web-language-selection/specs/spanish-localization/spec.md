## MODIFIED Requirements

### Requirement: Spanish UI localization
The system SHALL provide Spanish translations for user-facing Web UI navigation, page labels, forms, and action buttons in core workflows, and localized surfaces SHALL reflect the active culture selected by the user.

#### Scenario: Render contacts page in Spanish
- **WHEN** the active culture is Spanish
- **THEN** contacts page headings, labels, and actions are shown in Spanish

#### Scenario: Render localized navigation in Spanish after selection
- **WHEN** a user selects Spanish from the Web language selector
- **THEN** the navigation menu is rendered in Spanish after the redirect completes

### Requirement: Culture selection and fallback
The system SHALL resolve culture using explicit user selection first, then request language hints, and finally a configured default culture, and the Web language selector SHALL persist the user's chosen culture for later visits.

#### Scenario: Use explicit user language preference
- **WHEN** a user has selected Spanish as preferred language
- **THEN** the application renders and responds using Spanish regardless of browser header order

#### Scenario: Persist selected language for later visits
- **WHEN** a user selects a supported language from the Web language selector
- **THEN** the application stores that preference so subsequent visits use the same culture without selecting again

#### Scenario: Restore persisted language after application restart
- **WHEN** a user previously selected a supported language and the application restarts
- **THEN** the next visit uses the persisted culture instead of reverting to the default culture

#### Scenario: Fallback to default culture
- **WHEN** no supported language is resolved from user preference or request headers
- **THEN** the application uses the configured default culture

## ADDED Requirements

### Requirement: Language selector layout
The system SHALL present the Web language selector within the sidebar layout without overlapping or displacing navigation content on supported viewport sizes.

#### Scenario: Render selector without overlapping the menu
- **WHEN** the sidebar is rendered on a supported viewport
- **THEN** the language selector remains visually aligned within the sidebar header area
- **AND** the first navigation item is fully visible below the selector
