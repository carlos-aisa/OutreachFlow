# contact-group-management Specification

## Purpose
Reusable audiences for contact outreach.

## Requirements

### Requirement: Named contact groups
The system SHALL allow users to create, view, update, list, and delete named contact groups.

#### Scenario: Create manual group
- **WHEN** a user creates a group with a valid name and no criteria
- **THEN** the system persists a group whose membership is managed manually

### Requirement: Criteria-based group membership
The system SHALL support optional province, city, organization type, and contact-tag criteria for group membership.

#### Scenario: Values within a criterion use OR semantics
- **WHEN** a group selects Oviedo and Gijón as cities
- **THEN** a contact associated with an organization in either city matches the city criterion

#### Scenario: Active criteria use AND semantics
- **WHEN** a group selects cities Oviedo and Gijón and tags Primaria and Secundaria
- **THEN** a contact matches the group only when it matches a selected city and at least one selected tag

### Requirement: Manual group membership overrides
The system SHALL allow users to include or exclude individual contacts from a group regardless of criteria results.

#### Scenario: Exclude matching contact
- **WHEN** a user manually excludes a contact that matches the group criteria
- **THEN** the contact is not included in the group
