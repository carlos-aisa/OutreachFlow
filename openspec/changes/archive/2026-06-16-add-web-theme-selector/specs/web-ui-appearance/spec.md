## MODIFIED Requirements

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
The web application SHALL render a persistent navigation shell that clearly separates navigation from work content, makes the current location easy to identify, and provides a clear destination for user settings without embedding preference controls in the sidebar header.

#### Scenario: Active route is visually identifiable
- **WHEN** a user navigates to a page from the sidebar
- **THEN** the corresponding navigation item MUST show a stronger active state than inactive items

#### Scenario: Settings destination is available in navigation
- **WHEN** the sidebar navigation is rendered
- **THEN** the user MUST be able to access the settings page from the primary navigation items

#### Scenario: Active navigation state remains legible in dark theme
- **WHEN** the dark theme is active and a navigation item is selected
- **THEN** the selected item background, text, and icon MUST remain visually distinguishable from inactive items
