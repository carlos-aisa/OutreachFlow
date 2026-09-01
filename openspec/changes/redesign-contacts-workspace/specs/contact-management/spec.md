## MODIFIED Requirements

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
