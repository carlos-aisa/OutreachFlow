## ADDED Requirements

### Requirement: Campaign recipient follow-up context
The system SHALL associate a campaign-configured post-send follow-up task with the campaign recipient that caused it.

#### Scenario: Campaign recipient send creates follow-up
- **WHEN** a campaign recipient is sent successfully and campaign follow-up is enabled
- **THEN** the system creates the configured contact follow-up task and records its campaign recipient context
