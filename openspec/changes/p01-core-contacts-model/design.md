## Context

Phase 0 created the layered .NET solution, EF Core SQLite baseline, API, and Blazor shell. The next step is the core CRM model that every later outreach workflow depends on.

## Goals / Non-Goals

**Goals:**

- Model organizations, contacts, tags, and contact tags in a generic way.
- Keep business rules in Domain and orchestration in Application.
- Persist with EF Core SQLite using migrations.
- Expose REST endpoints with DTOs and OpenAPI documentation.
- Provide basic Blazor screens for repeated contact management work.

**Non-Goals:**

- Email templates, draft generation, sending, history, imports, and follow-up tasks.
- Organization tagging.
- Provider integrations with external contact systems.

## Decisions

- Treat `Contact`, `Organization`, and `Tag` as aggregate roots with one repository per aggregate root. This matches the repository rule in the backend standards and keeps future use cases explicit.
- Store `ContactTag` as a join entity owned by persistence and manipulated through contact/tag application use cases. This keeps the domain API simple while preserving many-to-many behavior.
- Enforce duplicate prevention by normalized email at the application and database levels. This gives a useful user-facing validation path and a database constraint as a final guard.
- Use enums for contact status. This avoids hardcoded strings in behavior while keeping the initial model simple.
- Keep Blazor pages thin and call application/API services instead of duplicating business rules in components.

## Risks / Trade-offs

- Email uniqueness may be too strict for shared inboxes. Mitigation: document the MVP rule and revisit if real use cases require scoped uniqueness.
- SQLite has limited concurrency. Mitigation: keep transactions short and design repository APIs that can move to PostgreSQL later.
- Early UI may be intentionally simple. Mitigation: prioritize correct flows and test coverage before visual polish.

## Migration Plan

- Add EF entities and mappings.
- Add migration for organizations, contacts, tags, and contact tags.
- Run integration tests against SQLite.
- Rollback by removing the migration before data is relied on.

## Open Questions

- None for MVP.
