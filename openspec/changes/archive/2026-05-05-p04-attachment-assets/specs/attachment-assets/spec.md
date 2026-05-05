## ADDED Requirements

### Requirement: Attachment asset management
The system SHALL allow users to register, upload, view, list, update metadata for, and deactivate reusable attachment assets.

#### Scenario: Upload attachment asset
- **WHEN** a valid file is uploaded with attachment metadata
- **THEN** the system stores the file and persists its metadata

#### Scenario: Deactivate attachment asset
- **WHEN** an attachment asset is deactivated
- **THEN** the asset remains stored but is excluded from active attachment selection

### Requirement: Attachment metadata
The system SHALL store attachment name, file name, content type, storage path, size, description, active state, and created timestamp.

#### Scenario: Persist metadata
- **WHEN** an attachment asset is created
- **THEN** the stored metadata describes the uploaded file without exposing unsafe file paths

### Requirement: Safe local storage
The system SHALL store attachment files under a configured local storage root.

#### Scenario: Reject unsafe path input
- **WHEN** a user submits file metadata containing path traversal input
- **THEN** the system rejects the unsafe path and does not store a file outside the configured root

### Requirement: Template default attachments
The system SHALL allow active attachment assets to be associated with email templates as default attachments.

#### Scenario: Add default template attachment
- **WHEN** an active attachment asset is added to a template
- **THEN** the template includes that attachment as a default for future draft generation

#### Scenario: Reject inactive default attachment
- **WHEN** an inactive attachment asset is added to a template
- **THEN** the system rejects the association
