## 1. Domain and Persistence

- [ ] 1.1 Add `FollowUpTask` domain model and type enum.
- [ ] 1.2 Add behavior for completion and completed timestamp.
- [ ] 1.3 Add EF Core mapping and due date/contact indexes.
- [ ] 1.4 Create and verify the EF Core migration.

## 2. Application

- [ ] 2.1 Add DTOs and use cases for follow-up task CRUD.
- [ ] 2.2 Add complete follow-up use case.
- [ ] 2.3 Add dashboard query support for pending follow-ups.
- [ ] 2.4 Add optional post-send follow-up creation setting and behavior.
- [ ] 2.5 Record follow-up activities.
- [ ] 2.6 Add application tests for task creation, completion, dashboard query, and auto creation.

## 3. API

- [ ] 3.1 Add REST endpoints for follow-up tasks.
- [ ] 3.2 Add dashboard data endpoint if needed by Web.
- [ ] 3.3 Update OpenAPI documentation.

## 4. Web

- [ ] 4.1 Add follow-up list and creation UI.
- [ ] 4.2 Add pending follow-ups to dashboard.
- [ ] 4.3 Add follow-ups section to contact detail.
- [ ] 4.4 Add complete action.

## 5. Verification

- [ ] 5.1 Add EF and API integration tests.
- [ ] 5.2 Run `dotnet test`.
- [ ] 5.3 Update README with follow-up behavior.
