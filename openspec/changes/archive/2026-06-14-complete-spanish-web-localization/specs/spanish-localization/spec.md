## MODIFIED Requirements

### Requirement: Spanish UI localization
The system SHALL provide Spanish translations for user-facing Web UI navigation, page labels, forms, action buttons, placeholders, loading states, and empty-state copy in supported OutreachFlow workflows, and localized surfaces SHALL reflect the active culture selected by the user.

#### Scenario: Render contacts page in Spanish
- **WHEN** the active culture is Spanish
- **THEN** contacts page headings, labels, and actions are shown in Spanish

#### Scenario: Render localized navigation in Spanish after selection
- **WHEN** a user selects Spanish from the Web language selector
- **THEN** the navigation menu is rendered in Spanish after the redirect completes

#### Scenario: Render dashboard summary in Spanish
- **WHEN** a Spanish-culture user opens the dashboard
- **THEN** dashboard headings, metric labels, and follow-up summary text are displayed in Spanish

### Requirement: Localization coverage for key workflows
The system SHALL include Spanish localization for the supported Web product workflows for dashboard, organizations, contacts, tags, sender profiles, templates, attachments, drafts, draft generation and detail, follow-up tasks, and imports.

#### Scenario: Open templates flow in Spanish
- **WHEN** a Spanish-culture user navigates to templates pages
- **THEN** list and form text in that workflow are displayed in Spanish

#### Scenario: Open draft generation flow in Spanish
- **WHEN** a Spanish-culture user navigates to draft generation
- **THEN** filter labels, step headings, attachment copy, preview copy, and action buttons in that workflow are displayed in Spanish

#### Scenario: Open follow-up tasks flow in Spanish
- **WHEN** a Spanish-culture user navigates to follow-up tasks
- **THEN** task creation labels, table headings, loading text, and empty-state text are displayed in Spanish

#### Scenario: Open imports flow in Spanish
- **WHEN** a Spanish-culture user navigates to imports
- **THEN** import actions, status labels, table headings, and progress messaging are displayed in Spanish
