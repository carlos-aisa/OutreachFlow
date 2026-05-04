## ADDED Requirements

### Requirement: Follow-up task management
The system SHALL allow users to create, view, update, list, and complete follow-up tasks for contacts.

#### Scenario: Create follow-up task
- **WHEN** a user creates a follow-up task for a contact with a due date
- **THEN** the system persists the task as incomplete

#### Scenario: Complete follow-up task
- **WHEN** a user marks a follow-up task complete
- **THEN** the system stores completion state and completed timestamp

### Requirement: Pending follow-up visibility
The system SHALL display pending follow-up tasks on the dashboard and contact detail screen.

#### Scenario: Dashboard shows pending follow-ups
- **WHEN** pending follow-up tasks exist
- **THEN** the dashboard displays upcoming tasks ordered by due date

### Requirement: Follow-up activity recording
The system SHALL record contact activity when follow-up tasks are created and completed.

#### Scenario: Record follow-up created activity
- **WHEN** a follow-up task is created
- **THEN** the system creates a `FollowUpCreated` activity for the contact

#### Scenario: Record follow-up completed activity
- **WHEN** a follow-up task is completed
- **THEN** the system creates a `FollowUpCompleted` activity for the contact

### Requirement: Optional post-send follow-up creation
The system SHALL support configurable automatic follow-up task creation after successful email sends.

#### Scenario: Auto follow-up disabled
- **WHEN** automatic follow-up creation is disabled and an email is sent
- **THEN** the system does not create a follow-up task automatically

#### Scenario: Auto follow-up enabled
- **WHEN** automatic follow-up creation is enabled and an email is sent
- **THEN** the system creates a follow-up task using the configured due date offset
