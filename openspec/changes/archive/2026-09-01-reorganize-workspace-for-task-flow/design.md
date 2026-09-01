## Context

This follows the same workflow-redesign review that produced `campaign-management`. It was validated against a two-screen mockup (new sidebar grouping + home queue, and a campaign detail screen) before being written up. The sidebar currently renders one flat `nav-group` of 12 items; the home page (`Home.razor`) currently shows summary metrics with no prioritized actions.

## Goals / Non-Goals

**Goals:**
- Group navigation by what a user is trying to do, not by which table the data lives in.
- Make "what should I do right now" answerable from the home page without navigating anywhere first.
- Ship the parts of this that don't depend on campaigns (nav regrouping, draft/follow-up queue widgets) without being blocked on `campaign-management`/`add-campaign-recipient-lifecycle` landing first.

**Non-Goals:**
- No page is removed, renamed at the route level, or has its own functionality changed — this only changes where each destination sits in the sidebar and what the home page shows.
- No change to how draft review, follow-ups, or (later) campaigns compute their own pending/overdue state — the queue only reads and links to that existing state.
- No dashboard analytics or historical charts — the critique was "shows metrics instead of prioritizing work," not "has no metrics at all"; existing counts stay, just de-emphasized.

## Decisions

**Relabel, don't reroute.** "Contactos" becomes "Directorio" in the nav label to fit inside the new Contactos group without repeating the group's own name, but the route (`/contacts`) and the page itself are unchanged. Every other destination keeps its existing route and label.

**The home page aggregates by calling existing API clients directly, the same way `DraftGeneration.razor` already injects several.** `Home.razor` calls `EmailDraftApiClient` (pending review count), `FollowUpTaskApiClient` (overdue/due-today), and — once it exists — a campaign API client (open campaigns and their candidate counts). No new aggregation endpoint; each widget is an independent query, matching how every other page in this app already composes itself from multiple typed API clients.

**The campaign widgets degrade to nothing, not to an error, when campaigns don't exist yet.** Since this change may ship before or independently of `campaign-management`, the "Campañas activas" section and any campaign-derived queue item simply don't render when there are no campaigns (or the capability isn't present) — the rest of the queue (drafts, follow-ups) still works standalone.

**Queue item ordering is fixed by category, not by a cross-category priority score.** Draft reviews, then follow-ups, then campaign candidates — in that order, always. A single scoring algorithm across unrelated work types would be guesswork dressed up as intelligence; fixed category order is honest about what the system actually knows (counts), not what it doesn't (which task is truly "most important" for this user today).

## Risks / Trade-offs

- Relabeling "Contactos" to "Directorio" inside a "Contactos" group could momentarily confuse users who search for the old label → Mitigation: the route and page content are unchanged, and "Directorio" reads clearly as "the list of contacts" once seen inside its group; revisit if this proves confusing in practice.
- A home page assembling four independent API calls adds a few round trips vs. one dashboard-summary endpoint → Accepted; matches the app's existing pattern everywhere else, and a dedicated aggregation endpoint would be premature optimization for a handful of lightweight list/count calls.
- Fixed category ordering could deprioritize something genuinely urgent in a category listed later (e.g., one overdue follow-up buried under ten routine draft reviews) → Mitigation: each category still shows its own count prominently, so nothing is hidden, only ordered; revisit if usage shows this matters.

## Open Questions

(none outstanding — sequencing, aggregation approach, and empty-state behavior were resolved above.)
