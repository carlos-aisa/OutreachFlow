## MODIFIED Requirements

### Requirement: Named contact groups
The system SHALL allow users to create, view, update, list, and delete named contact groups. Every group SHALL define at least one membership criterion; the system SHALL reject a create or update request that specifies no criteria.

#### Scenario: Reject group with no criteria
- **WHEN** a user submits a create or update request for a group with an empty criteria list
- **THEN** the system rejects the request with a validation error and does not persist the change

### Requirement: Criteria-based group membership
The system SHALL require at least one province, city, organization type, or contact-tag criterion for group membership.

#### Scenario: Values within a criterion use OR semantics
- **WHEN** a group selects Oviedo and Gijón as cities
- **THEN** a contact associated with an organization in either city matches the city criterion

#### Scenario: Active criteria use AND semantics
- **WHEN** a group selects cities Oviedo and Gijón and tags Primaria and Secundaria
- **THEN** a contact matches the group only when it matches a selected city and at least one selected tag

## ADDED Requirements

### Requirement: Contact group list filtering and size visibility
The contact groups list page SHALL let users filter groups by name and SHALL show each group's current contact count as a badge next to its name.

#### Scenario: Filter groups by name
- **WHEN** a user enters text into the group name filter
- **THEN** the list shows only groups whose name contains that text, and the list's count badge reflects the filtered result

#### Scenario: Show member count badge per group
- **WHEN** the contact groups list renders
- **THEN** each group displays a badge with its current number of member contacts

### Requirement: Contact group membership view
The contact group detail page SHALL default its status filter to showing current members, SHALL classify a contact as a current member only when its status is member-by-criteria or member-by-manual-inclusion, and SHALL show a contact-count summary that reflects the currently filtered/visible contacts rather than a fixed total.

#### Scenario: Default view shows current members
- **WHEN** a user opens a contact group's detail page
- **THEN** the status filter defaults to "Members" and only contacts that are current members are shown

#### Scenario: Manually excluded contact is absent from the members filter
- **WHEN** the status filter is set to "Members"
- **THEN** a contact with an excluded-manually status does not appear in the list

#### Scenario: Manually excluded contact appears in the non-members filter
- **WHEN** the status filter is set to "Non-members"
- **THEN** a contact with an excluded-manually status appears in the list

#### Scenario: Summary badge follows the active filter
- **WHEN** a user changes the status filter or the search text
- **THEN** the contact-count summary badge updates to reflect the number of contacts currently visible under that filter, out of the group's total contact pool
