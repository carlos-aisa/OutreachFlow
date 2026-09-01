## MODIFIED Requirements

### Requirement: Safe local storage
The system SHALL store attachment files under a configured local storage root and SHALL only back up or restore attachment files that resolve inside that configured root.

#### Scenario: Reject unsafe path input
- **WHEN** a user submits file metadata containing path traversal input
- **THEN** the system rejects the unsafe path and does not store a file outside the configured root

#### Scenario: Include stored attachment files in runtime backup
- **WHEN** an operator runs the supported runtime data backup workflow
- **THEN** the backup package includes attachment files from the configured local storage root

#### Scenario: Reject attachment restore outside configured root
- **WHEN** a restore package would place an attachment file outside the configured local storage root
- **THEN** the restore workflow rejects the package and does not write files outside the configured root
