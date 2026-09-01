## Why

A product review (with a validated mockup) found that the sidebar lists all 12 destinations at one flat level, forcing a new or non-technical user to understand the app's internal data model before they can find anything, and that the home page shows metrics without ever telling the user what to do next. Both are navigation/orientation problems the app's own visual-system spec already commits to solving, just not yet in these two places.

## What Changes

- Regroup the sidebar into four task-oriented sections instead of one flat list: **Inicio**; **Contactos** (Directorio, Grupos de contactos, Seguimientos, Importar contactos); **Campañas** (Campañas, Borradores por revisar); **Configuración** (Plantillas, Cuentas de envío, Adjuntos, Etiquetas, Ajustes). No page is removed — every current destination gets a new home in one of these groups.
- Rebuild the home page around a prioritized "qué hacer ahora" work queue — pending draft reviews, overdue/upcoming follow-ups, and (once available) campaigns with new eligible candidates — as the page's primary content, each item linking straight to the action. Existing summary counts move to a small, secondary line rather than the page's hero content.
- Add a "Campañas activas" section to the home page summarizing each open campaign's recipient progress, linking to its detail screen.

## Capabilities

### New Capabilities
(none)

### Modified Capabilities
- `web-ui-appearance`: the navigation-shell requirement gains task-grouped sidebar sections (replacing the current single flat group); a new requirement establishes the home page as a prioritized work queue instead of a passive metrics view.

## Impact

- Affected code: `NavMenu.razor`/`.razor.css` (grouped sections, no page removed or renamed at the routing level except relabeling "Contactos" as "Directorio" within the new Contactos group), and `Home.razor` (rebuilt around the work queue).
- The pending-draft-review and due-follow-up queue widgets read existing `email-draft-review` and `follow-up-tasks` data — no requirement changes to either, just a new UI consumer.
- The "campañas activas" home widget and the "Campañas"/"Borradores por revisar" nav destinations depend on `campaign-management` and `add-campaign-recipient-lifecycle` existing; the rest of this change (nav regrouping, draft/follow-up queue widgets) has no such dependency and can ship independently if sequencing requires it.
- No domain or API contract changes.
- Requires updated Blazor component tests and localization strings for the regrouped navigation and the rebuilt home page.
