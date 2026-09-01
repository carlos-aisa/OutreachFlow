## MODIFIED Requirements

### Requirement: Record creation uses a dismissible side panel on primary list pages
The web application SHALL let a primary list page open record creation and editing in a dismissible side panel triggered from a primary header action or a record's edit action, keeping the record list as the page's default, dominant content.

#### Scenario: Primary action opens the creation panel
- **WHEN** a user selects the primary "New" action in a primary list page's header
- **THEN** the system opens a side panel containing the creation form without navigating away from the list

#### Scenario: List remains the default view
- **WHEN** a user opens a primary list page
- **THEN** the record list is visible without first requiring the user to dismiss or scroll past a creation form

#### Scenario: Edit action opens the same panel pre-filled
- **WHEN** a user selects a record's edit action on a page that supports editing
- **THEN** the system opens the same side panel pre-filled with that record's data instead of swapping an inline form between create and edit modes

#### Scenario: Page-scoped secondary content remains outside the panel
- **WHEN** a page has content tied to the whole page or a set of related records rather than the single record's own fields, such as a contact group's member list or a template's variable reference
- **THEN** that content MUST remain on the main page rather than move into the panel

#### Scenario: Pattern applies across primary list pages
- **WHEN** a user opens the Organizations, Tags, Contact groups, Sender profiles, Templates, Attachments, or Follow-ups page
- **THEN** each page follows the shared header-action, filters-above-list, and side-panel-creation pattern
