## MODIFIED Requirements

### Requirement: Contact management UI
The system SHALL provide Blazor screens for listing contacts and creating contacts through a contact-first intake flow that makes organization association optional and supports existing or newly entered organizations.

#### Scenario: Create contact from UI
- **WHEN** a user submits the contact intake form with valid data
- **THEN** the UI creates the contact and presents a contextual next action

#### Scenario: Create contact and organization from UI
- **WHEN** a user submits valid contact data and inline organization data
- **THEN** the UI creates the associated records without requiring navigation to the organizations page
