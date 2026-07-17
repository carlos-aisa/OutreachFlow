# campaign-management Specification

## Purpose
Persistent, non-linear preparation for periodic outreach.

## Requirements

### Requirement: Persistent campaign preparation
The system SHALL allow users to create, save, list, update, and resume campaigns in a partially completed preparation state.

#### Scenario: Save incomplete campaign
- **WHEN** a user saves a campaign that has a name but lacks some sending prerequisites
- **THEN** the system persists the campaign as in preparation without generating drafts or sending email

### Requirement: Campaign readiness guidance
The system SHALL show which prerequisites are missing before a user can generate campaign drafts.

#### Scenario: Incomplete campaign
- **WHEN** a campaign lacks a message, sender profile, or recipients
- **THEN** the workspace identifies the missing prerequisite and prevents draft generation
