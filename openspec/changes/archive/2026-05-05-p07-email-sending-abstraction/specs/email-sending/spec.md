## ADDED Requirements

### Requirement: Email sender abstraction
The system SHALL send emails through an application-level email sender abstraction.

#### Scenario: Send through fake provider
- **WHEN** the configured provider is the fake sender
- **THEN** the system returns a successful fake provider result without contacting an external service

### Requirement: Approved draft sending
The system SHALL allow sending only approved drafts.

#### Scenario: Send approved draft
- **WHEN** an approved draft with valid recipient, sender, body, and active attachments is sent
- **THEN** the system sends the email and marks the draft as `Sent`

#### Scenario: Reject unapproved draft
- **WHEN** a draft that is not approved is submitted for sending
- **THEN** the system rejects the send request

### Requirement: Sending safety rules
The system SHALL block sending to do-not-contact contacts, invalid email recipients, drafts with unresolved variables, inactive attachments, or already sent drafts.

#### Scenario: Reject do-not-contact send
- **WHEN** an approved draft belongs to a contact marked do-not-contact
- **THEN** the system rejects the send request

#### Scenario: Reject duplicate draft send
- **WHEN** a draft has already been sent
- **THEN** the system rejects any later send request for the same draft

### Requirement: Email message persistence
The system SHALL persist an email message record for send attempts.

#### Scenario: Persist successful send
- **WHEN** a send succeeds
- **THEN** the system creates an email message with provider, provider message id, sent timestamp, and `Sent` status

#### Scenario: Persist failed send
- **WHEN** a send fails
- **THEN** the system creates an email message with provider, failure reason, and `Failed` status

### Requirement: Contact last contacted update
The system SHALL update the contact last contacted timestamp after a successful send.

#### Scenario: Successful send updates contact
- **WHEN** an email is sent successfully
- **THEN** the contact `LastContactedAt` value is updated to the send timestamp
