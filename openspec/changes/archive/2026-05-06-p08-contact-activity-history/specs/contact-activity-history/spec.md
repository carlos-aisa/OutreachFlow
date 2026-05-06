## ADDED Requirements

### Requirement: Contact activity records
The system SHALL persist activity records for relevant contact events.

#### Scenario: Record contact creation
- **WHEN** a contact is created
- **THEN** the system creates a `ContactCreated` activity for that contact

#### Scenario: Record email sent
- **WHEN** an email send succeeds
- **THEN** the system creates an `EmailSent` activity for that contact

#### Scenario: Record email failed
- **WHEN** an email send fails
- **THEN** the system creates an `EmailFailed` activity for that contact

### Requirement: Activity metadata
The system SHALL allow activity records to include optional subject, body preview, metadata JSON, and occurred timestamp.

#### Scenario: Persist email activity metadata
- **WHEN** an email activity is created
- **THEN** the activity includes subject and body preview values when available

### Requirement: Contact activity query
The system SHALL allow users to list activities for a contact in reverse chronological order.

#### Scenario: Query contact timeline
- **WHEN** a user requests activities for a contact
- **THEN** the system returns that contact's activities ordered by newest first

### Requirement: Contact detail timeline UI
The system SHALL display contact activity history on the contact detail screen.

#### Scenario: View contact timeline
- **WHEN** a user opens contact detail
- **THEN** the UI displays the contact activity timeline
