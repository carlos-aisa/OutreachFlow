## Why

The current OutreachFlow web application works functionally, but its visual presentation still feels close to the default Blazor/Bootstrap baseline. That makes dense screens harder to scan and reduces the sense of polish in the main workflows people use every day.

## What Changes

- Refresh the web application's visual language around a dark navigation shell and a bright work canvas
- Improve sidebar hierarchy, active states, and language selector placement so navigation feels clearer and more intentional
- Refine shared page structure across dashboard, forms, cards, and tables so dense workflows are easier to scan
- Standardize button, form, and table presentation across the main web pages without changing business workflows
- Apply the refreshed visual system first to the main layout and navigation, then to high-traffic pages such as dashboard, contacts, contact detail, templates, and related operational pages

## Capabilities

### New Capabilities
- `web-ui-appearance`: Defines the shared visual system, navigation behavior, layout hierarchy, and page presentation requirements for the Blazor web application

### Modified Capabilities
- None

## Impact

- Affected code: `src/OutreachFlow.Web/Components/Layout/*`, `src/OutreachFlow.Web/Components/Pages/*`, `src/OutreachFlow.Web/wwwroot/app.css`
- Affected systems: Blazor web UI only
- Dependencies: existing Bootstrap-based styling and current localization-aware navigation controls
