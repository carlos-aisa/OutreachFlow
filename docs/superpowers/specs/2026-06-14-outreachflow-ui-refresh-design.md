# OutreachFlow UI Refresh Design

Date: 2026-06-14
Status: Approved for review
Scope: Frontend visual refresh only

## Summary

This design refreshes the OutreachFlow web application to feel easier to use and more visually intentional without changing the existing product structure or workflows. The chosen direction is `Focus Canvas / Balanced C`: a dark, stable navigation column paired with a bright work canvas, clearer hierarchy, refined spacing, and disciplined use of a vivid accent color.

The goal is not a redesign of product behavior. The goal is to make the current application feel more coherent, polished, and pleasant during long work sessions while preserving the existing Blazor layout and feature pages.

## Product Subject

OutreachFlow is a working tool for people managing contacts, templates, follow-ups, drafts, and imports. The interface should feel:

- Clear under daily operational use
- Fast to scan
- Professional without looking sterile
- Attractive without sacrificing legibility

The intended personality is a modern control surface, not a marketing site and not a generic Bootstrap admin template.

## Design Direction

### Chosen direction

`Focus Canvas / Balanced C`

Core characteristics:

- Dark navigation shell with strong contrast and visual stability
- Light content canvas with generous whitespace
- Clean, modern typography and restrained visual noise
- Vivid accent used only for primary actions, active states, and important status cues

### Why this direction

This direction best balances usability and visual appeal. It creates a clear separation between navigation and work areas, improves orientation across dense screens, and adds a more premium identity without making the application feel flashy or exhausting.

## Visual System

### Color palette

Use a restrained palette with distinct roles:

- `Midnight Navy` `#182033`
  Primary sidebar and structural dark surfaces
- `Slate Ink` `#27324A`
  Secondary dark surfaces, dividers, hover states
- `Canvas Mist` `#F4F7FB`
  Main application background
- `Paper White` `#FFFFFF`
  Cards, forms, tables, elevated surfaces
- `Signal Green` `#8BD85A`
  Primary call to action, active indicators, selected emphasis
- `Steel Gray` `#96A0B5`
  Supporting text, subtle UI separators, inactive controls

Accent usage must stay disciplined. The application should remain mostly dark-structure plus light-canvas, with green reserved for focus and action.

### Typography

Typography should support clarity first, personality second.

- Display/page headings: a more expressive but still practical sans serif
- Body/forms/tables: a neutral, highly legible sans serif
- Small labels/supporting metadata: the body family with tighter scale and stronger weight

The implementation may use web-safe fallbacks if introducing custom fonts is not practical in the first pass, but hierarchy must still be visibly improved through size, weight, and spacing.

### Shape and elevation

- Use medium radii consistently across cards, inputs, and buttons
- Keep shadows soft and shallow
- Prefer surface contrast over heavy borders
- Avoid glossy gradients in work surfaces

The interface should feel precise, not decorative.

## Global Layout

### Sidebar

The sidebar becomes the visual anchor of the application.

Changes:

- Replace the current demo-like blue-purple gradient treatment with a flatter dark navigation shell
- Increase brand presence at the top
- Integrate the language selector as a first-class control inside the sidebar header
- Improve spacing rhythm between groups and links
- Strengthen active navigation state with a clearer selected background and accent cue
- Keep hover states subtle and readable

The sidebar must feel stable and intentional on desktop, and compact but still usable on smaller screens.

### Top row

The current top row should stop acting like a generic placeholder bar.

Changes:

- Remove visual prominence from the current "About" pattern
- Simplify the bar so it serves page context rather than decoration
- Reduce noise and reclaim more attention for the page content

### Main canvas

The content area should feel more spacious and systematic.

Changes:

- Increase visual breathing room between sections
- Use more consistent page width and inner padding
- Make cards feel like part of one system rather than repeated Bootstrap defaults

## Page-Level Design Rules

### Shared page structure

Most work pages already follow a repeatable pattern:

1. Page title and short description
2. Filter or form card
3. Table or detail card

This structure should remain, but its presentation should improve:

- Page headings should gain stronger hierarchy
- Descriptions should become quieter supporting text
- Primary cards should have more consistent spacing and visual weight
- Secondary content should look intentionally subordinate

### Dashboard

The dashboard should become a clearer operational summary.

Changes:

- Refine metric cards so they read as one family
- Improve contrast between metric label and value
- Make the pending follow-ups section feel like part of the same visual system

### Contacts

This is a high-value page and should receive the most visible improvement.

Changes:

- Make the filters card easier to scan and less cramped
- Make the create-contact form feel more guided
- Refine the contacts table to reduce raw Bootstrap appearance
- Improve count badge and action prominence

### Contact detail

This screen should feel more editorial and structured.

Changes:

- Turn the summary, tags, activity, and follow-up sections into clearly separated information blocks
- Improve density and readability of small tables
- Make the tag assignment area clearer and more action-oriented

### Templates, Sender Profiles, Organizations, Tags, Follow-Ups, Imports, Drafts

These screens should adopt the same shared visual language:

- Consistent page title treatment
- Consistent card headers
- Better input sizing and spacing
- Cleaner table presentation
- Primary and secondary actions visually standardized

## Component Styling Strategy

This refresh should prefer shared visual rules over page-by-page one-off styling.

Implementation strategy:

- Add global tokens and shared surface/button/form/table rules in the web stylesheet
- Update `MainLayout` and `NavMenu` styling first
- Apply a small number of reusable page-level utility classes where needed
- Avoid introducing a new component library
- Avoid large structural rewrites of existing Razor pages

This keeps the work additive and low-risk.

## Interaction Rules

### Buttons

- Primary buttons should use the accent color and stand out clearly
- Secondary buttons should be quieter and rely on outline or neutral surface treatment
- Button sizing should feel deliberate and consistent across pages

### Forms

- Inputs should be slightly taller and easier to scan
- Labels should be clearer and less visually lost
- Focus states should feel modern and consistent with the accent system

### Tables

- Table headers should be lighter and cleaner
- Row spacing should improve scanability
- Hover states should be subtle
- Links inside tables should remain easy to identify

## Responsiveness

The refresh must remain fully usable on desktop and mobile.

Requirements:

- Sidebar collapse behavior must remain reliable
- The language selector must stay correctly placed in both narrow and wide layouts
- Form grids must stack cleanly
- Tables must remain readable inside responsive wrappers
- Spacing reductions on mobile must preserve hierarchy, not flatten it

## Accessibility

The refresh must preserve or improve accessibility.

Requirements:

- Maintain sufficient color contrast for dark navigation and light content surfaces
- Keep visible keyboard focus states
- Preserve semantic HTML already present in forms and tables
- Avoid color-only communication for active or important states
- Respect reduced motion if any animation is introduced

## Scope and Non-Goals

### In scope

- Visual styling refresh
- Layout polish
- Navigation polish
- Shared page hierarchy improvements
- Better consistency across cards, forms, and tables

### Not in scope

- Workflow redesign
- Data model changes
- API changes
- New component framework adoption
- Major page restructuring beyond what is needed for styling clarity

## Implementation Priority

Recommended order:

1. Global visual tokens in `wwwroot/app.css`
2. `MainLayout` visual cleanup
3. `NavMenu` redesign
4. Shared page spacing and typography rules
5. Dashboard polish
6. Contacts and Contact Detail polish
7. Remaining work pages aligned to the same visual system

## Testing and Verification

Verification should focus on visible behavior and layout consistency.

Required checks:

- Build succeeds
- Existing localization behavior remains intact
- Sidebar language selector still persists and remains positioned correctly
- Desktop layout works across main pages
- Mobile layout remains usable for navigation, forms, and tables
- No regressions in component tests that depend on the updated layout markup

Manual QA should review:

- Sidebar active/hover/focus states
- Form readability and spacing
- Table readability
- Contact and template workflows after the visual changes

## Risks

- Over-styling shared Bootstrap primitives could create regressions across many pages
- Making the sidebar too visually dominant could hurt focus on work content
- Introducing custom fonts too early could complicate the first implementation pass

Mitigation:

- Start with tokens and layout surfaces
- Use the accent color sparingly
- Prefer incremental visual refinement over aggressive restyling

## Acceptance Criteria

This design is successful when:

- The application feels visually coherent across all primary work pages
- Navigation feels more premium and easier to orient within
- Dense pages like Contacts and Contact Detail feel calmer and easier to scan
- The app still feels fast and practical, not ornamental
- The visual refresh can be implemented without changing business workflows
