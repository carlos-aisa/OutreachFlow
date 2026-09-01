# web-ui-appearance Specification

## Purpose
Define the shared visual expectations for the OutreachFlow web workspace so layout, navigation, forms, tables, and actions feel consistent across the primary user flows.
## Requirements
### Requirement: Shared visual system for the web workspace
The web application SHALL present a consistent visual system across layout, cards, forms, tables, actions, and settings surfaces so that primary work pages feel like one product instead of isolated Bootstrap defaults, and that visual system SHALL remain coherent in each supported theme.

#### Scenario: Common surfaces render with consistent hierarchy
- **WHEN** a user opens any primary web page such as dashboard, contacts, templates, follow-ups, imports, or settings
- **THEN** the page MUST use the shared workspace styling for background surfaces, card treatment, spacing rhythm, and action hierarchy

#### Scenario: Primary and secondary actions remain visually distinct
- **WHEN** a page renders more than one action type
- **THEN** primary actions MUST be visually emphasized more strongly than secondary or destructive actions using the shared action styling rules

#### Scenario: Theme maintains legible contrast
- **WHEN** the active theme changes between light and dark modes
- **THEN** text, icons, surfaces, borders, and interactive states MUST remain visually legible with the shared workspace styling

### Requirement: Navigation shell establishes clear orientation
The web application SHALL render a persistent navigation shell that clearly separates navigation from work content, groups destinations by the task they support rather than by their underlying data model, makes the current location easy to identify, provides a clear destination for user settings without embedding preference controls in the sidebar header, and keeps the sidebar header itself compact.

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

#### Scenario: Navigation is grouped by task
- **WHEN** the sidebar navigation is rendered
- **THEN** destinations MUST appear under named groups reflecting what the user is trying to do (an "Inicio" destination, a "Contactos" group for finding and maintaining contacts, a "Campañas" group for messaging and its results, and a "Configuración" group for reusable setup such as templates, sender accounts, attachments, and tags) rather than as one undifferentiated list

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

### Requirement: Layout remains usable across responsive breakpoints
The web application SHALL preserve the refreshed appearance without reducing usability on narrow screens.

#### Scenario: Mobile navigation remains usable
- **WHEN** the viewport switches to the collapsed navigation layout
- **THEN** users MUST still be able to access navigation items and the language selector without layout overlap or hidden controls

#### Scenario: Responsive forms and tables remain operable
- **WHEN** a page containing forms or tables is viewed on a narrow screen
- **THEN** controls MUST remain reachable, stacked content MUST preserve hierarchy, and tabular content MUST remain readable inside responsive containers

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

### Requirement: Single-record pickers support search at scale
The web application SHALL let a user find a single existing contact or organization by typing to search, rather than scrolling a plain dropdown listing every record, anywhere a form assigns or filters by one existing contact or organization.

#### Scenario: Assign a contact by searching
- **WHEN** a user opens the follow-up task creation panel
- **THEN** they can type to filter contacts by name or email and select one, instead of scrolling a full list

#### Scenario: Filter by organization by searching
- **WHEN** a user filters contacts, or filters draft-generation recipients, by organization
- **THEN** they can type to filter organizations by name and select one, instead of scrolling a full list

### Requirement: Home page prioritizes actionable work
The web application SHALL make the home page's primary content a prioritized queue of work that needs the user's attention, rather than passive summary metrics.

#### Scenario: Pending work is surfaced first
- **WHEN** a user opens the home page and drafts are awaiting review or follow-ups are overdue or due today
- **THEN** those items appear as the page's primary content, each with a direct action to address it, above any summary counts

#### Scenario: Summary counts remain available but secondary
- **WHEN** a user opens the home page
- **THEN** aggregate counts (such as total contacts or organizations) remain visible but are visually subordinate to the actionable work queue

#### Scenario: Empty queue states are clear
- **WHEN** a user opens the home page and no draft reviews, follow-ups, or campaign candidates are pending
- **THEN** the page states plainly that there is nothing pending rather than showing an empty or broken section

