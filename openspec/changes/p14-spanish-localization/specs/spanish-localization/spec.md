## ADDED Requirements

### Requirement: Spanish UI localization
The system SHALL provide Spanish translations for user-facing Web UI navigation, page labels, forms, and action buttons in core workflows.

#### Scenario: Render contacts page in Spanish
- **WHEN** the active culture is Spanish
- **THEN** contacts page headings, labels, and actions are shown in Spanish

### Requirement: Spanish validation and error messages
The system SHALL provide Spanish localized messages for user-facing validation and known API error responses.

#### Scenario: Return Spanish validation message
- **WHEN** a request fails a known validation rule and active culture is Spanish
- **THEN** the response message is returned in Spanish

### Requirement: Culture selection and fallback
The system SHALL resolve culture using explicit user selection first, then request language hints, and finally a configured default culture.

#### Scenario: Use explicit user language preference
- **WHEN** a user has selected Spanish as preferred language
- **THEN** the application renders and responds using Spanish regardless of browser header order

#### Scenario: Fallback to default culture
- **WHEN** no supported language is resolved from user preference or request headers
- **THEN** the application uses the configured default culture

### Requirement: Localization coverage for key workflows
The system SHALL include Spanish localization for organization, contacts, tags, sender profiles, and templates user flows.

#### Scenario: Open templates flow in Spanish
- **WHEN** a Spanish-culture user navigates to templates pages
- **THEN** list and form text in that workflow are displayed in Spanish
