## ADDED Requirements

### Requirement: Settings-based runtime data recovery
The system SHALL expose runtime data backup and restore actions from the web settings experience so users can manage recovery from a dedicated configuration surface.

#### Scenario: Open runtime data actions from settings
- **WHEN** a user opens the settings page
- **THEN** the application MUST provide a data management section that exposes backup and restore actions

#### Scenario: Backup action produces a portable package
- **WHEN** a user starts a runtime data backup from the settings page
- **THEN** the system MUST generate a backup package that contains the runtime database file, the attachment storage contents, and backup metadata

### Requirement: Runtime data restore validation
The system SHALL validate restore packages before any live runtime data is replaced.

#### Scenario: Reject incompatible restore package
- **WHEN** a user uploads a package that is missing required files or backup metadata
- **THEN** the restore workflow MUST fail before modifying the live runtime data

#### Scenario: Reject restore into invalid runtime target
- **WHEN** the restore workflow resolves a database or attachment destination outside the configured runtime root
- **THEN** the system MUST reject the restore and leave the live runtime data unchanged

### Requirement: Confirmed runtime data recovery
The system SHALL require explicit confirmation before replacing live runtime data from a validated restore package.

#### Scenario: Confirm restore after successful validation
- **WHEN** a restore package has been validated successfully
- **THEN** the system MUST require a separate explicit confirmation before applying the recovery operation

#### Scenario: Cancel restore after validation
- **WHEN** a user cancels a validated restore before confirmation
- **THEN** the system MUST leave the live runtime data unchanged

### Requirement: Atomic recovery of database and attachments
The system SHALL restore the database and attachment files as one coordinated recovery operation.

#### Scenario: Restore runtime data from validated package
- **WHEN** a user confirms recovery from a validated package
- **THEN** the system MUST replace the live runtime database and attachment files from the same package before reporting success

#### Scenario: Restore failure does not report partial success
- **WHEN** a recovery operation cannot complete successfully
- **THEN** the workflow MUST report the failure and MUST NOT report that runtime data was restored successfully
