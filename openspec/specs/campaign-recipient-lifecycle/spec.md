# campaign-recipient-lifecycle Specification

## Purpose
TBD - created by archiving change add-campaign-recipient-lifecycle. Update Purpose after archive.
## Requirements
### Requirement: Campaign recipient history
The system SHALL persist a recipient record for each contact explicitly incorporated into a campaign and track its candidate, draft, approval, sent, excluded, or failed lifecycle state.

#### Scenario: Incorporate eligible contact
- **WHEN** a user explicitly incorporates an eligible contact into an open campaign
- **THEN** the system creates a campaign recipient record without sending an email

### Requirement: Late-recipient discovery
The system SHALL show eligible contacts that match an open campaign's selected audience sources but are not yet campaign recipients.

#### Scenario: New matching contact
- **WHEN** a new eligible contact matches the audience of an open campaign and has no recipient record
- **THEN** the campaign shows the contact as available for explicit incorporation

### Requirement: Per-message-version duplicate prevention
The system SHALL prevent more than one campaign recipient delivery workflow for the same campaign, contact, and message version.

#### Scenario: Already incorporated contact
- **WHEN** a user attempts to incorporate a contact already associated with the campaign's current message version
- **THEN** the system rejects the duplicate and preserves the existing recipient history

### Requirement: Campaign safety revalidation
The system SHALL revalidate recipient eligibility before generating or sending campaign drafts.

#### Scenario: Contact becomes do-not-contact
- **WHEN** an incorporated recipient is marked do-not-contact before generation or sending
- **THEN** the system excludes the recipient from that operation and does not send email

