## 1. Sidebar regrouping

- [x] 1.1 Restructure `NavMenu.razor` into four named groups (Inicio; Contactos: Directorio/Grupos de contactos/Seguimientos/Importar contactos; Campañas: Campañas/Borradores por revisar; Configuración: Plantillas/Cuentas de envío/Adjuntos/Etiquetas/Ajustes), relabeling "Contactos" to "Directorio" without changing its route. Also added "Organizaciones" (Contactos group) and "Generación de borradores" (Campañas group) — both are live destinations not named in the mockup's shorthand list; placed per the same "no page removed" non-goal, with Draft Generation's placement confirmed with the user via AskUserQuestion.
- [x] 1.2 Update `NavMenu.razor.css` for multiple named groups (spacing between groups, section labels), and add/adjust nav icons for the new "Campañas" and "Seguimientos"-under-Contactos placement. Also fixed a pre-existing missing `.bi-collection-fill-nav-menu` icon mask (Grupos de contactos) surfaced while touching this file.
- [x] 1.3 Update localization strings for the new/changed nav labels and section names (`Nav.Dashboard`, `Nav.Contacts`, `Nav.Imports`, `Nav.Drafts`, `Nav.SenderProfiles`, new `NavSection.*` keys) in both `SharedResource.resx` and `SharedResource.es.resx`. Removed the now-unused `Layout.Workspace` key (sidebar header no longer shows a tagline).

## 2. Home work queue

- [x] 2.1 Rebuilt `Home.razor` to query pending draft reviews (`EmailDraftApiClient`) and due/overdue follow-ups (`FollowUpTaskApiClient`), rendering them as a prioritized queue (`.queue-list`/`.queue-row`) with direct action links, above the campaign and summary sections.
- [x] 2.2 Added an "Active campaigns" section reading open campaigns (`CampaignApiClient`), their recipients (`CampaignRecipientApiClient.ListAsync`) for Sent/Pending stats, and candidate counts (`CampaignRecipientApiClient.DiscoverCandidatesAsync`); the section and any campaign-derived queue item simply don't render when there are no open campaigns.
- [x] 2.3 Added an explicit empty-state message (`Home.Queue.Empty`) when no queue items exist.
- [x] 2.4 Moved existing summary counts (contacts/organizations/groups) into a single de-emphasized `.stat-strip` line below the campaigns section.

## 3. Verification

- [x] 3.1 Added `HomeComponentTests.cs` (empty state, fixed-order queue rendering with counts/links, active-campaigns section absent/present with stats) and updated `WebLocalizationComponentTests.cs` (nav groups in Spanish, `Home` render with the new required API clients). No dedicated nav-active-state test was added beyond the existing `ShouldMarkCurrentNavigationItemAsActive`, which still covers the active-route scenario against the regrouped markup.
- [x] 3.2 Not performed by the agent — flagged to the user for manual verification in the running app.
- [x] 3.3 Ran the full solution build and test suite (`dotnet build`/`dotnet test` on `OutreachFlow.sln`, excluding the pre-existing `DevelopmentHostingConfigurationTests` redirect-output artifact): 279 tests passed, 0 failed.
