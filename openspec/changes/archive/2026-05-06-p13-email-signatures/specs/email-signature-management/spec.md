## ADDED Requirements

### Requirement: Sender signature persistence
The system SHALL allow sender profiles to store an optional signature with an explicit format.

#### Scenario: Store HTML signature
- **WHEN** a user creates or updates a sender profile with signature format Html and non-empty signature content
- **THEN** the system persists both signature format and signature content with the profile

#### Scenario: Store RTF signature
- **WHEN** a user creates or updates a sender profile with signature format Rtf and non-empty signature content
- **THEN** the system persists both signature format and signature content with the profile

### Requirement: Signature format validation
The system SHALL reject signatures that do not declare a supported format or provide invalid signature payload.

#### Scenario: Reject unsupported signature format
- **WHEN** a sender profile request contains a signature format outside the supported values Html and Rtf
- **THEN** the system rejects the request with a validation error

#### Scenario: Reject signature content without format
- **WHEN** a sender profile request contains signature content but no signature format
- **THEN** the system rejects the request with a validation error

### Requirement: Signature append in draft composition
The system SHALL append sender signature content to the end of generated email body content when a signature exists.

#### Scenario: Append signature for sender with signature
- **WHEN** an email draft is generated using a sender profile with signature content
- **THEN** the resulting draft body ends with the sender signature content

#### Scenario: Keep body unchanged when signature absent
- **WHEN** an email draft is generated using a sender profile without signature content
- **THEN** the resulting draft body contains only the rendered template body

### Requirement: Signature management UI behavior
The system SHALL provide sender profile UI fields to edit signature format and signature content.

#### Scenario: Edit sender signature in web UI
- **WHEN** a user opens sender profile create or edit form
- **THEN** the UI allows selecting Html or Rtf and entering signature content for persistence
