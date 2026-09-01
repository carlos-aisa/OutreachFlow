## MODIFIED Requirements

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

## ADDED Requirements

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
