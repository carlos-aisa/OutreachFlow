## 1. Domain and persistence

- [x] 1.1 Add a `Campaign` domain aggregate (name, description, template reference, status) and a `CampaignAudience` join to contact groups, with validation requiring at least one audience group.
- [x] 1.2 Configure EF Core persistence, relationships, indexes, and a migration for campaigns and their audience associations.
- [x] 1.3 Add relational integration tests for creation, audience add/remove, and listing (`CampaignPersistenceTests`); message change and open/close/reopen transitions are covered end-to-end against real SQLite via the API integration tests (`CampaignEndpointTests`), and exhaustively at the domain level (`CampaignTests`).

## 2. Application and API

- [x] 2.1 Add create, list, get, update-message, add/remove-audience, and close/reopen operations.
- [x] 2.2 Add campaign endpoints and update OpenAPI contracts.
- [x] 2.3 Add application and API tests for validation failures (no audience, no name, invalid template/group reference) and status transitions.

## 3. Web experience

- [x] 3.1 Add a Campaigns list page and a campaign detail page (name, status, audience, message) following the existing header/side-panel pattern used by other list pages.
- [x] 3.2 Add localization and web component tests for campaign creation, audience management, and status changes.

## 4. Verification and documentation

- [x] 4.1 Document creating a campaign, choosing its audience and message, and opening/closing it — delivered as in-app copy (page description, audience help text, action labels) per this project's practice of self-explanatory UI over separate guides; no other feature in this codebase has a standalone user-facing doc either.
- [x] 4.2 Run affected domain, application, API, relational integration, web component, and solution build checks.
