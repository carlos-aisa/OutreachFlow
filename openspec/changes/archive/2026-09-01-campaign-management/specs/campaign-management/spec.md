## ADDED Requirements

### Requirement: Campaign creation
The system SHALL allow users to create a campaign with a name, an optional purpose description, one message (an existing email template), and one or more target audiences (existing contact groups).

#### Scenario: Create a campaign
- **WHEN** a user submits a valid campaign name, an existing template, and at least one existing contact group
- **THEN** the system persists the campaign as open with created and updated timestamps

#### Scenario: Reject campaign without an audience
- **WHEN** a campaign is submitted with no contact group selected
- **THEN** the system rejects the request with a validation error

#### Scenario: Reject campaign without a name
- **WHEN** a campaign is submitted without a name
- **THEN** the system rejects the request with a validation error

### Requirement: Campaign message
The system SHALL let a campaign reference exactly one current email template as its message, changeable at any time.

#### Scenario: Change a campaign's message
- **WHEN** a user updates an open campaign to reference a different existing template
- **THEN** the system persists the new template reference without altering previously generated drafts

### Requirement: Campaign audience
The system SHALL let a campaign reference one or more existing contact groups as its audience, addable and removable while the campaign is open.

#### Scenario: Add a group to an open campaign's audience
- **WHEN** a user adds an existing contact group to an open campaign
- **THEN** the system persists the association and the group's members become discoverable as campaign candidates

#### Scenario: Remove a group from a campaign's audience
- **WHEN** a user removes a contact group from a campaign's audience
- **THEN** the system persists the removal without affecting recipients already incorporated into the campaign

### Requirement: Campaign status
The system SHALL support open and closed campaign status, changeable explicitly by the user, with no automatic transitions.

#### Scenario: Close a campaign
- **WHEN** a user closes an open campaign
- **THEN** the system marks the campaign closed and the campaign stops accepting new candidate incorporation

#### Scenario: Reopen a campaign
- **WHEN** a user reopens a closed campaign
- **THEN** the system marks the campaign open again

### Requirement: Campaign listing and detail view
The system SHALL provide Blazor screens for listing campaigns and viewing a single campaign's name, status, audience, and message.

#### Scenario: List campaigns
- **WHEN** a user opens the Campaigns page
- **THEN** the UI shows all campaigns with their name and status

#### Scenario: View campaign detail
- **WHEN** a user opens a specific campaign
- **THEN** the UI shows its name, status, audience groups, and current message
