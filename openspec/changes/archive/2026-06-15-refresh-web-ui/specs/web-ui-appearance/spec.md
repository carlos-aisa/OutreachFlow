## ADDED Requirements

### Requirement: Shared visual system for the web workspace
The web application SHALL present a consistent visual system across layout, cards, forms, tables, and actions so that primary work pages feel like one product instead of isolated Bootstrap defaults.

#### Scenario: Common surfaces render with consistent hierarchy
- **WHEN** a user opens any primary web page such as dashboard, contacts, templates, follow-ups, or imports
- **THEN** the page MUST use the shared workspace styling for background surfaces, card treatment, spacing rhythm, and action hierarchy

#### Scenario: Primary and secondary actions remain visually distinct
- **WHEN** a page renders more than one action type
- **THEN** primary actions MUST be visually emphasized more strongly than secondary or destructive actions using the shared action styling rules

### Requirement: Navigation shell establishes clear orientation
The web application SHALL render a persistent navigation shell that clearly separates navigation from work content and makes the current location easy to identify.

#### Scenario: Active route is visually identifiable
- **WHEN** a user navigates to a page from the sidebar
- **THEN** the corresponding navigation item MUST show a stronger active state than inactive items

#### Scenario: Language selector remains integrated in navigation
- **WHEN** the sidebar header is rendered on desktop or mobile layouts
- **THEN** the language selector MUST remain placed inside the navigation shell without overlapping the brand or navigation controls

### Requirement: Primary work pages support fast scanning
The web application SHALL improve the visual hierarchy of dense operational pages so users can scan titles, filters, forms, and tabular content with less effort.

#### Scenario: Page headers establish clear structure
- **WHEN** a user opens a primary work page
- **THEN** the page title, supporting description, and first actionable section MUST appear in a clear top-to-bottom hierarchy

#### Scenario: Dense content remains readable
- **WHEN** a page contains forms, tables, or stacked detail sections
- **THEN** spacing, typography, and surface treatment MUST preserve readable grouping between controls, records, and supporting metadata

### Requirement: Layout remains usable across responsive breakpoints
The web application SHALL preserve the refreshed appearance without reducing usability on narrow screens.

#### Scenario: Mobile navigation remains usable
- **WHEN** the viewport switches to the collapsed navigation layout
- **THEN** users MUST still be able to access navigation items and the language selector without layout overlap or hidden controls

#### Scenario: Responsive forms and tables remain operable
- **WHEN** a page containing forms or tables is viewed on a narrow screen
- **THEN** controls MUST remain reachable, stacked content MUST preserve hierarchy, and tabular content MUST remain readable inside responsive containers
