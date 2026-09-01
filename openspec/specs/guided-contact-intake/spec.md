# guided-contact-intake Specification

## Purpose

Provide a contact-first entry flow that removes unnecessary navigation between contacts and organizations while preserving controlled, explicit data entry.

## Requirements

### Requirement: Contact-first intake
The system SHALL provide a contact-first intake flow that allows a user to save a contact with no organization, an existing organization, or a newly entered organization without leaving the flow.

#### Scenario: Save independent contact
- **WHEN** a user submits valid contact details without selecting or entering an organization
- **THEN** the system creates the contact without an organization association

#### Scenario: Save contact with a new organization
- **WHEN** a user submits valid contact details and valid inline organization details
- **THEN** the system creates the organization and associated contact as one successful operation

### Requirement: Contextual next action
The intake flow SHALL show the saved contact and offer a direct next action to continue managing that contact or begin campaign-related work.

#### Scenario: Contact saved
- **WHEN** a contact is created successfully
- **THEN** the user sees confirmation and a direct action related to the created contact
