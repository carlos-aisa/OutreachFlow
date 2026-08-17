## ADDED Requirements

### Requirement: Persistent campaign preparation
The system SHALL allow users to create, save, list, update, and resume campaigns in a partially completed preparation state.

#### Scenario: Save incomplete campaign
- **WHEN** a user saves a campaign that has a name but lacks some sending prerequisites
- **THEN** the system persists the campaign as in preparation without generating drafts or sending email

### Requirement: Non-linear campaign workspace
The system SHALL allow users to configure campaign message, audience, sender profile, attachments, and follow-up settings in any order.

#### Scenario: Start with message
- **WHEN** a user creates or edits a campaign message before selecting recipients
- **THEN** the system saves the message and keeps the campaign in preparation

#### Scenario: Start with audience
- **WHEN** a user selects groups or contacts before configuring the campaign message
- **THEN** the system saves the audience selection and keeps the campaign in preparation

### Requirement: Campaign readiness guidance
The system SHALL show which prerequisites are missing before a user can generate campaign drafts.

#### Scenario: Incomplete campaign
- **WHEN** a campaign lacks a message, sender profile, or recipients
- **THEN** the workspace identifies the missing prerequisite and prevents draft generation
