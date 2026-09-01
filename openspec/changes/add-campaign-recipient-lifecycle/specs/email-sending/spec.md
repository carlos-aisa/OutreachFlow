## ADDED Requirements

### Requirement: Campaign send context
The system SHALL persist campaign recipient context for a campaign-owned draft send attempt and update the recipient delivery state from the persisted outcome.

#### Scenario: Send campaign draft successfully
- **WHEN** an approved campaign-owned draft is sent successfully
- **THEN** the system records the send and marks the corresponding campaign recipient as sent

#### Scenario: Campaign draft send fails
- **WHEN** a campaign-owned draft send fails
- **THEN** the system records the failure and marks the corresponding campaign recipient as failed
