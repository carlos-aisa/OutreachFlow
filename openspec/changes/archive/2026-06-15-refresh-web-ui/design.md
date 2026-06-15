## Context

OutreachFlow already has the functional page structure needed for its CRM and outreach workflows, but the web experience still inherits much of the default Blazor and Bootstrap visual language. The current layout uses a gradient sidebar, placeholder-like top bar, and mostly stock card, form, and table styling. That creates a gap between the maturity of the workflows and the perceived quality of the product.

This refresh is cross-cutting because it touches shared layout files, global stylesheet rules, and multiple page implementations. The work must preserve the current Blazor Web App structure, existing localization behavior, and responsive Bootstrap-based page composition while improving the visual hierarchy across the primary screens.

Stakeholders are users working daily with contacts, templates, drafts, follow-ups, and imports, plus maintainers who need the refresh to stay incremental and low-risk.

## Goals / Non-Goals

**Goals:**
- Introduce a shared visual system for the Blazor web application
- Replace the current default-feeling layout treatment with a more intentional navigation shell and work canvas
- Improve scanability on dense pages such as Contacts, Contact Detail, Templates, and other operational screens
- Keep the implementation additive and compatible with the existing Razor page structure
- Preserve accessibility, localization behavior, and responsive usability

**Non-Goals:**
- Redesign business workflows or page-level feature behavior
- Introduce a new UI framework or component library
- Rewrite pages into a new component architecture unless a small reusable styling helper is clearly justified
- Change API contracts, backend behavior, or persistence logic

## Decisions

### Decision: Use a token-driven global stylesheet refresh anchored in `wwwroot/app.css`

The refresh will introduce shared color, spacing, radius, typography, and surface rules in the global stylesheet instead of scattering one-off visual fixes across pages.

Rationale:
- The existing UI already relies on common Bootstrap class patterns, so global styling gives the broadest improvement with the smallest structural disruption
- Shared tokens create consistency across many pages without requiring deep markup rewrites
- The approach is easy to evolve incrementally

Alternatives considered:
- Page-by-page scoped CSS only: rejected because it would create inconsistent styling and duplicate rules
- Replacing Bootstrap styling wholesale: rejected because it increases blast radius and risk

### Decision: Keep the current layout structure but redesign `MainLayout` and `NavMenu`

The layout will continue to use the current `MainLayout` plus sidebar navigation composition, but with updated markup and CSS to establish the new visual identity.

Rationale:
- These files are the highest-leverage place to change the perceived product quality
- They already own the navigation shell, language selector, and global page framing
- Preserving the layout structure avoids unnecessary routing or rendering risk

Alternatives considered:
- Building a completely new shell component hierarchy: rejected because it adds architectural churn without product benefit

### Decision: Treat Bootstrap as the structural layer, not the final visual language

Existing Bootstrap classes and responsive layout utilities will remain in place where useful, but the visual styling will be overridden to create a more product-specific appearance.

Rationale:
- Bootstrap still provides reliable grid, spacing, and responsive behaviors
- Overriding the look is lower risk than replacing the structure
- This matches the repository standard of keeping scope tight and avoiding unnecessary framework changes

Alternatives considered:
- Removing Bootstrap conventions from page markup: rejected for the first implementation pass because it would increase change volume without improving behavior

### Decision: Roll out in layers from shared shell to highest-traffic pages

Implementation will proceed from global tokens and layout to the most frequently used pages, then to the rest of the work pages.

Rationale:
- Shared shell changes immediately improve the entire application
- Contacts and Contact Detail are the densest high-value screens and will show the largest usability gain
- Layered rollout simplifies verification and isolates regressions

Alternatives considered:
- Styling all pages equally in one pass: rejected because it would make verification harder and slow iteration

### Decision: Preserve localization and responsive behavior as first-class constraints

The visual refresh will explicitly protect the sidebar language selector, mobile navigation collapse behavior, and readable responsive tables/forms.

Rationale:
- Localization persistence and selector placement recently required bug fixes and cannot regress
- The new visual direction uses stronger contrast and more deliberate spacing, which can introduce overlap or compression if responsiveness is not treated as a constraint

Alternatives considered:
- Styling desktop first and fixing mobile later: rejected because layout regressions would be expensive to unwind

## Risks / Trade-offs

- [Global overrides affect many pages] → Mitigation: introduce tokens first, then verify changed primitives on representative pages before broadening
- [The dark sidebar may become visually heavy] → Mitigation: keep hover and active states disciplined, and let the content canvas stay brighter and calmer
- [Bootstrap overrides may create edge-case spacing regressions] → Mitigation: prefer targeted shared selectors and add page-level helper classes only when necessary
- [Layout markup changes may break tests or selector assumptions] → Mitigation: preserve stable IDs and important interaction hooks, then run relevant component and integration tests
- [Custom typography may complicate the first pass] → Mitigation: treat font changes as optional unless they can be added cleanly without slowing the core refresh

## Migration Plan

1. Add shared visual tokens and base overrides in `src/OutreachFlow.Web/wwwroot/app.css`
2. Refresh `MainLayout` and `NavMenu` markup/CSS while preserving navigation behavior and localization controls
3. Apply shared page hierarchy rules to dashboard and the main work pages
4. Refine Contacts and Contact Detail as the primary dense-screen targets
5. Align Templates, Sender Profiles, Organizations, Tags, Drafts, Follow-Ups, Attachments, and Imports to the same system
6. Run targeted tests for localization-aware navigation and representative web component/integration coverage
7. Perform manual desktop and mobile verification

Rollback strategy:
- Revert the change set at the web layer only; no data or API migration is involved

## Open Questions

- Whether the first pass should introduce custom web fonts or rely on improved hierarchy using existing/system fonts
- Whether a small reusable page-header helper class is sufficient, or whether one or two shared Razor components would reduce duplication without adding unnecessary abstraction
