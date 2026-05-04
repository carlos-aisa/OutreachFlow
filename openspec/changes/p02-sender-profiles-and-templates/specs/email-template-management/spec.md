## ADDED Requirements

### Requirement: Email template management
The system SHALL allow users to create, view, update, list, and deactivate email templates.

#### Scenario: Create email template
- **WHEN** a valid template name, subject template, and body template are submitted
- **THEN** the system persists the template as active by default

#### Scenario: Deactivate email template
- **WHEN** a template is deactivated
- **THEN** the template remains stored but is excluded from active template selection

### Requirement: Template variable guidance
The system SHALL expose the supported template variables through a centralized application contract.

#### Scenario: List supported variables
- **WHEN** a user opens the template editor
- **THEN** the UI displays the supported variables from the centralized contract

### Requirement: Generic reusable template content
The system SHALL store template content as configurable data and SHALL NOT hardcode business-specific outreach content.

#### Scenario: Store generic template
- **WHEN** a user creates a template with custom subject and body text
- **THEN** the exact user-provided content is persisted for later rendering
