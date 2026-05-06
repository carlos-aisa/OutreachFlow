## ADDED Requirements

### Requirement: CSV import preview
The system SHALL parse CSV contact files and show a preview before creating contacts.

#### Scenario: Preview valid CSV
- **WHEN** a valid CSV file is submitted for preview
- **THEN** the system returns parsed rows, validation results, and duplicate indicators without persisting contacts

### Requirement: CSV validation
The system SHALL validate imported contact rows before commit.

#### Scenario: Invalid row reported
- **WHEN** an import row is missing required contact data
- **THEN** the preview marks that row invalid with a validation message

### Requirement: Duplicate detection
The system SHALL detect existing contacts by normalized email during import preview and commit.

#### Scenario: Duplicate row detected
- **WHEN** an import row contains an email that already exists
- **THEN** the preview marks the row as duplicate and the commit does not create a duplicate contact

### Requirement: Tag assignment during import
The system SHALL allow selected existing tags to be assigned to contacts created by an import.

#### Scenario: Import with selected tags
- **WHEN** an import is committed with selected tag ids
- **THEN** each newly created contact receives those tags

### Requirement: Import job tracking
The system SHALL persist import job metadata including file name, status, row counts, created count, duplicate count, invalid count, and timestamps.

#### Scenario: Import job completed
- **WHEN** an import commit completes
- **THEN** the system stores an import job with final counts and completed status

### Requirement: Import UI
The system SHALL provide a Blazor CSV import flow with preview and commit steps.

#### Scenario: Commit from UI preview
- **WHEN** a user reviews a preview and confirms import
- **THEN** the UI commits valid non-duplicate rows and displays import results
