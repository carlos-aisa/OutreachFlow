## ADDED Requirements

### Requirement: Generate drafts from template
The system SHALL generate personalized email drafts for selected contacts using an email template and sender profile.

#### Scenario: Generate draft successfully
- **WHEN** a selected contact, active template, and valid sender profile are submitted
- **THEN** the system creates a draft with rendered subject and body

### Requirement: Draft render diagnostics
The system SHALL store render diagnostics on generated drafts.

#### Scenario: Draft needs review for missing variable
- **WHEN** rendering finds a missing variable value
- **THEN** the system creates the draft with `NeedsReview` status and stores the missing variable details

#### Scenario: Draft needs review for unknown variable
- **WHEN** rendering finds an unknown variable
- **THEN** the system creates the draft with `NeedsReview` status and stores the unknown variable details

### Requirement: Contact eligibility for draft generation
The system SHALL exclude contacts marked do-not-contact or without a valid email from draft generation.

#### Scenario: Exclude do-not-contact contact
- **WHEN** a selected contact is marked do-not-contact
- **THEN** the system does not generate a draft for that contact and reports the skipped contact

### Requirement: Draft attachment selection
The system SHALL copy template default attachments and selected optional attachments to generated drafts.

#### Scenario: Copy default attachments
- **WHEN** a template has active default attachments
- **THEN** generated drafts include those attachments

#### Scenario: Reject inactive attachment
- **WHEN** an inactive attachment is selected for draft generation
- **THEN** the system rejects the generation request

### Requirement: Draft generation UI
The system SHALL provide a Blazor draft generation wizard.

#### Scenario: Generate drafts from wizard
- **WHEN** a user completes recipient, template, sender, attachment, and preview steps
- **THEN** the UI creates drafts and shows the generation result
