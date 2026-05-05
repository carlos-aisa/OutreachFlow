## ADDED Requirements

### Requirement: Draft listing and detail
The system SHALL allow users to list drafts by status and view draft details.

#### Scenario: List drafts by status
- **WHEN** a user requests drafts with `NeedsReview` status
- **THEN** the system returns only drafts that need review

### Requirement: Draft editing
The system SHALL allow users to edit draft subject and body before approval.

#### Scenario: Edit draft content
- **WHEN** a user saves new subject and body content for a draft that is not sent or cancelled
- **THEN** the system persists the edited content

### Requirement: Draft approval
The system SHALL allow users to approve only drafts that have no unresolved variables and no render errors.

#### Scenario: Approve valid draft
- **WHEN** a draft has no unresolved variables or render errors
- **THEN** the system marks the draft as `Approved` and stores the approval timestamp

#### Scenario: Reject approval with unresolved variable
- **WHEN** a draft body still contains an unresolved `{{...}}` token
- **THEN** the system rejects approval

### Requirement: Draft cancellation
The system SHALL allow users to cancel drafts that have not been sent.

#### Scenario: Cancel unsent draft
- **WHEN** a user cancels a draft that is not sent
- **THEN** the system marks the draft as `Cancelled`

#### Scenario: Reject cancelling sent draft
- **WHEN** a user tries to cancel a sent draft
- **THEN** the system rejects the request

### Requirement: Human review UI
The system SHALL provide Blazor screens for draft listing and draft detail review.

#### Scenario: Approve draft from UI
- **WHEN** a user reviews a valid draft and clicks approve
- **THEN** the UI marks the draft as approved and refreshes its status
