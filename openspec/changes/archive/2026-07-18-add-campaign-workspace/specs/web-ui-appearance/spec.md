## MODIFIED Requirements

### Requirement: Navigation shell establishes clear orientation
The web application SHALL render a persistent navigation shell that clearly separates navigation from work content, makes the current location easy to identify, provides a clear destination for user settings, and groups primary work around contacts, campaigns, review, and follow-up tasks.

#### Scenario: Active route is visually identifiable
- **WHEN** a user navigates to a page from the sidebar
- **THEN** the corresponding navigation item MUST show a stronger active state than inactive items

#### Scenario: Settings destination is available in navigation
- **WHEN** the sidebar navigation is rendered
- **THEN** the user MUST be able to access the settings page from the primary navigation items

#### Scenario: Active navigation state remains legible in dark theme
- **WHEN** the dark theme is active and a navigation item is selected
- **THEN** the selected item background, text, and icon MUST remain visually distinguishable from inactive items

#### Scenario: Campaign work is discoverable
- **WHEN** a user opens the workspace navigation
- **THEN** campaigns and the work required to review and follow up on them MUST be grouped as primary tasks rather than configuration destinations
