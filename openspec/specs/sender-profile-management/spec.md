# sender-profile-management Specification

## Purpose
TBD - created by archiving change p02-sender-profiles-and-templates. Update Purpose after archive.
## Requirements
### Requirement: Sender profile management
The system SHALL allow users to create, view, update, list, and deactivate sender profiles.

#### Scenario: Create sender profile
- **WHEN** a valid sender profile is submitted
- **THEN** the system persists sender identity fields and timestamps

#### Scenario: Reject sender profile without email
- **WHEN** a sender profile is submitted without an email address
- **THEN** the system rejects the request with a validation error

### Requirement: Default sender profile
The system SHALL allow exactly one sender profile to be marked as default.

#### Scenario: Set default sender profile
- **WHEN** a sender profile is marked as default
- **THEN** the system clears the default flag from all other sender profiles

#### Scenario: Get default sender profile
- **WHEN** a default sender profile exists
- **THEN** the system returns that profile for workflows that require a default sender

### Requirement: Configurable sender identity
The system SHALL use sender profile data instead of hardcoded sender identity values.

#### Scenario: Sender profile includes signature
- **WHEN** a sender profile with a signature is stored
- **THEN** the signature is available to template and draft workflows

