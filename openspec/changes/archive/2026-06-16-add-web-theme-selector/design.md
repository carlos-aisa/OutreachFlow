## Context

OutreachFlow Web already has a shared visual system built around CSS custom properties in `wwwroot/app.css`, and it already persists the active culture through `wwwroot/js/culture.js`. Right now, however, the language selector lives in the sidebar header and there is no dedicated place for application preferences. At the same time, the user wants theme selection and does not want these controls embedded directly in the menu.

This change therefore becomes broader than a simple dark-mode addition. It introduces a settings destination in the navigation model, moves language selection into that page, adds theme selection alongside it, and keeps the whole experience coherent across localized workflows.

## Goals / Non-Goals

**Goals:**
- Add a dedicated settings page to the web workspace.
- Move language selection from the sidebar header into the settings page.
- Add light, dark, and system theme selection to the same settings page.
- Persist language and theme preferences across navigation and application restarts.
- Preserve accessibility and legibility across the navigation shell and shared workspace components.

**Non-Goals:**
- Adding backend user-profile storage for preferences.
- Creating multiple settings categories beyond the initial general preferences scope.
- Redesigning unrelated workflows beyond what is required for settings integration and theme support.
- Supporting arbitrary custom palettes or branding controls.

## Decisions

### Introduce a dedicated settings route instead of embedding preference controls in the sidebar header

- **Decision:** Add a navigation destination for settings and move preference controls out of the sidebar header into a proper page-level surface.
- **Why:** Preferences are easier to discover, translate, and extend when they live in a configuration page rather than inside the navigation chrome. This also keeps the sidebar visually focused on navigation.
- **Alternatives considered:**
  - **Keep controls in the sidebar header:** simpler, but repeats the current layout tension and leaves little room for future settings.
  - **Use a modal or flyout:** faster to open, but weaker for extensibility and less consistent with a growing settings surface.

### Group language and theme under a general preferences section

- **Decision:** The first version of the settings page should focus on a small general section that contains language and theme controls together.
- **Why:** Both are presentation preferences, both are persisted client-side, and both affect the overall workspace rather than a single feature.
- **Alternatives considered:**
  - **Separate pages for language and theme:** unnecessary fragmentation for the current scope.
  - **Put language in settings but leave theme in the sidebar:** inconsistent mental model and duplicated preference locations.

### Keep client-side persistence for both preferences

- **Decision:** Continue using browser-side persistence for language and add a sibling helper for theme persistence.
- **Why:** The application already uses client-side persistence successfully for culture, and theme is also a presentation preference that does not require a backend data model.
- **Alternatives considered:**
  - **Store preferences on the server:** more durable across browsers, but introduces backend scope and identity concepts the product does not currently have.
  - **Use URL-based preferences:** visible but noisy and not appropriate for sticky user settings.

### Apply theme through shared CSS custom properties

- **Decision:** Continue using shared CSS custom properties and switch the token set through a theme marker such as a `data-theme` attribute.
- **Why:** The current workspace styling is already token-based, so changing tokens is safer and more maintainable than branching per component.
- **Alternatives considered:**
  - **Separate stylesheet per theme:** workable, but easier to drift over time.
  - **Component-by-component conditional styling:** scattered and more likely to miss edge cases.

### Resolve theme before the interactive shell settles

- **Decision:** The theme helper should resolve and apply the active theme as early as possible during page load.
- **Why:** Theme flicker is especially noticeable in a layout-heavy application with a persistent sidebar and large page surfaces.
- **Alternatives considered:**
  - **Apply only after Blazor interactivity starts:** simpler, but creates a visible mismatch during initial render and forced reloads.

## Risks / Trade-offs

- **[Settings becomes a dumping ground for unrelated controls]** -> Keep the first version narrowly scoped to general preferences only.
- **[Theme and language persistence can interfere during full reload]** -> Keep storage keys independent and verify both selectors together.
- **[Dark mode introduces contrast regressions in selected nav states or dense pages]** -> Add explicit theme-aware tokens for active navigation, borders, and icon colors instead of relying on inherited defaults.
- **[Moving language out of the sidebar changes an existing interaction path]** -> Add a clear settings nav entry and keep the preferences section prominent on the new page.

## Migration Plan

1. Add the settings route and navigation entry without changing preference behavior yet.
2. Move the language selector into the new general preferences section.
3. Add theme preference persistence and dark-theme token overrides.
4. Verify that language changes, theme changes, and page navigation all preserve the selected preferences.

## Open Questions

- None at proposal time; the page structure is clear enough to proceed into implementation planning.
