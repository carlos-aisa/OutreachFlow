# contact-management Specification

## Purpose
TBD - created by archiving change p01-core-contacts-model. Update Purpose after archive.
## Requirements
### Requirement: Organization management
The system SHALL allow users to create, view, update, list, and delete generic organizations.

#### Scenario: Create organization
- **WHEN** a valid organization name and optional metadata are submitted
- **THEN** the system persists the organization with created and updated timestamps

#### Scenario: Reject organization without name
- **WHEN** an organization is submitted without a name
- **THEN** the system rejects the request with a validation error

### Requirement: Contact management
The system SHALL allow users to create, view, update, list, and delete contacts with optional organization association.

#### Scenario: Create contact with organization
- **WHEN** a valid contact is submitted with an existing organization id
- **THEN** the system persists the contact and associates it with that organization

#### Scenario: Create contact without organization
- **WHEN** a valid contact is submitted without an organization id
- **THEN** the system persists the contact as an independent contact

### Requirement: Contact status and do-not-contact controls
The system SHALL support contact status values and a do-not-contact flag.

#### Scenario: Mark contact as do not contact
- **WHEN** a contact is updated with do-not-contact enabled
- **THEN** the persisted contact reflects the do-not-contact state

#### Scenario: Change contact status
- **WHEN** a valid contact status is submitted
- **THEN** the system persists the new status

### Requirement: Contact email uniqueness
The system SHALL prevent duplicate contacts with the same normalized email address.

#### Scenario: Duplicate email rejected
- **WHEN** a new contact is submitted with an email already used by another contact
- **THEN** the system rejects the request and does not create a duplicate contact

### Requirement: Tag management
The system SHALL allow users to create, view, update, list, and delete flexible contact tags.

#### Scenario: Create tag
- **WHEN** a valid tag name and optional category are submitted
- **THEN** the system persists the tag

#### Scenario: Duplicate tag rejected
- **WHEN** a tag is submitted with a name that already exists in the same category
- **THEN** the system rejects the request

### Requirement: Contact tagging
The system SHALL allow users to assign and remove tags for contacts.

#### Scenario: Assign tag to contact
- **WHEN** an existing tag is assigned to an existing contact
- **THEN** the contact includes the tag exactly once

#### Scenario: Remove tag from contact
- **WHEN** an assigned tag is removed from a contact
- **THEN** the contact no longer includes that tag

### Requirement: Contact filtering
The system SHALL allow users to filter contacts by search text, tag, status, do-not-contact state, organization, and last contacted date.

#### Scenario: Filter contacts by tag and status
- **WHEN** the user lists contacts with a tag filter and a status filter
- **THEN** the system returns only contacts matching both filters

### Requirement: Contact management UI
The system SHALL provide a Blazor contacts page that shows the contact list as the default view and lets users create contacts through a side panel that keeps organization association optional and resolves an organization from a single search-or-create combobox, supporting an existing organization, a newly created organization, or no organization.

#### Scenario: Create contact from UI
- **WHEN** a user opens the New contact panel and submits valid contact data
- **THEN** the UI creates the contact, closes the panel, and presents a contextual next action

#### Scenario: Create contact with an existing organization
- **WHEN** a user selects an existing organization from the organization combobox and submits valid contact data
- **THEN** the UI creates the contact and associates it with the selected organization without navigating away from the contacts page

#### Scenario: Create contact with a new organization
- **WHEN** a user types an organization name that does not match an existing organization, chooses to create it, and submits valid contact data
- **THEN** the UI creates the organization and the associated contact as one operation without navigating to the organizations page

#### Scenario: Cancel discards panel input
- **WHEN** a user opens the New contact panel, enters data, and cancels instead of submitting
- **THEN** the UI closes the panel without creating a contact or organization and discards the entered data

### Requirement: Contact group visibility
The system SHALL show a contact's group memberships in contact management and allow group membership to be managed from the relevant contact or group experience.

#### Scenario: View contact groups
- **WHEN** a user views a contact that belongs to groups
- **THEN** the user can identify those groups and manage manual membership where permitted
