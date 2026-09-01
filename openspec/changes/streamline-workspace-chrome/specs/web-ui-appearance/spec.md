## MODIFIED Requirements

### Requirement: Navigation shell establishes clear orientation
The web application SHALL render a persistent navigation shell that clearly separates navigation from work content, makes the current location easy to identify, provides a clear destination for user settings without embedding preference controls in the sidebar header, and keeps the sidebar header itself compact.

#### Scenario: Active route is visually identifiable
- **WHEN** a user navigates to a page from the sidebar
- **THEN** the corresponding navigation item MUST show a stronger active state than inactive items

#### Scenario: Settings destination is available in navigation
- **WHEN** the sidebar navigation is rendered
- **THEN** the user MUST be able to access the settings page from the primary navigation items

#### Scenario: Active navigation state remains legible in dark theme
- **WHEN** the dark theme is active and a navigation item is selected
- **THEN** the selected item background, text, and icon MUST remain visually distinguishable from inactive items

#### Scenario: Sidebar header stays compact
- **WHEN** the sidebar renders
- **THEN** its header shows only the product name, without a workspace tagline or descriptive paragraph competing with navigation items for space

### Requirement: Primary work pages support fast scanning
The web application SHALL improve the visual hierarchy of dense operational pages, including record-creation panels opened from them, so users can scan titles, filters, forms, tabular content, and contextual next actions with less effort.

#### Scenario: Page headers establish clear structure
- **WHEN** a user opens a primary work page
- **THEN** the page shows one title with a purpose-specific description directly beneath it, without a separate top bar above it or a small line repeating the same title

#### Scenario: Dense content remains readable
- **WHEN** a page contains forms, tables, or stacked detail sections
- **THEN** spacing, typography, and surface treatment MUST preserve readable grouping between controls, records, and supporting metadata

#### Scenario: Contact intake reveals optional work progressively
- **WHEN** a user opens the New contact panel and chooses to create a new organization from the combobox
- **THEN** essential contact fields remain visible while the optional extra organization fields stay collapsed behind an explicit action, without implying that organization association is required

## ADDED Requirements

### Requirement: Single-record pickers support search at scale
The web application SHALL let a user find a single existing contact or organization by typing to search, rather than scrolling a plain dropdown listing every record, anywhere a form assigns or filters by one existing contact or organization.

#### Scenario: Assign a contact by searching
- **WHEN** a user opens the follow-up task creation panel
- **THEN** they can type to filter contacts by name or email and select one, instead of scrolling a full list

#### Scenario: Filter by organization by searching
- **WHEN** a user filters contacts, or filters draft-generation recipients, by organization
- **THEN** they can type to filter organizations by name and select one, instead of scrolling a full list
