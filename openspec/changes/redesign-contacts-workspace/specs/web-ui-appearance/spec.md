## MODIFIED Requirements

### Requirement: Primary work pages support fast scanning
The web application SHALL improve the visual hierarchy of dense operational pages, including record-creation panels opened from them, so users can scan titles, filters, forms, tabular content, and contextual next actions with less effort.

#### Scenario: Page headers establish clear structure
- **WHEN** a user opens a primary work page
- **THEN** the page title, supporting description, and first actionable section MUST appear in a clear top-to-bottom hierarchy

#### Scenario: Dense content remains readable
- **WHEN** a page contains forms, tables, or stacked detail sections
- **THEN** spacing, typography, and surface treatment MUST preserve readable grouping between controls, records, and supporting metadata

#### Scenario: Contact intake reveals optional work progressively
- **WHEN** a user opens the New contact panel and chooses to create a new organization from the combobox
- **THEN** essential contact fields remain visible while the optional extra organization fields stay collapsed behind an explicit action, without implying that organization association is required

## ADDED Requirements

### Requirement: Record creation uses a dismissible side panel on primary list pages
The web application SHALL let a primary list page open record creation in a dismissible side panel triggered from a primary action in the page header, keeping the record list as the page's default, dominant content.

#### Scenario: Primary action opens the creation panel
- **WHEN** a user selects the primary "New" action in a primary list page's header
- **THEN** the system opens a side panel containing the creation form without navigating away from the list

#### Scenario: List remains the default view
- **WHEN** a user opens a primary list page
- **THEN** the record list is visible without first requiring the user to dismiss or scroll past a creation form
