## ADDED Requirements

### Requirement: Campaign recipient draft generation
The system SHALL generate drafts for explicitly incorporated campaign recipients and report their resulting recipient lifecycle state.

#### Scenario: Generate campaign recipient drafts
- **WHEN** an open campaign with valid prerequisites generates drafts for eligible incorporated recipients
- **THEN** the system creates one draft per eligible recipient and marks each recipient as awaiting review or approval according to render diagnostics
