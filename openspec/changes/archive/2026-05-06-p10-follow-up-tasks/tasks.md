## 1. Domain and Persistence

- [x] 1.1 Add `FollowUpTask` domain model and type enum.
- [x] 1.2 Add behavior for completion and completed timestamp.
- [x] 1.3 Add EF Core mapping and due date/contact indexes.
- [x] 1.4 Create and verify the EF Core migration.

## 2. Application

- [x] 2.1 Add DTOs and use cases for follow-up task CRUD.
- [x] 2.2 Add complete follow-up use case.
- [x] 2.3 Add dashboard query support for pending follow-ups.
- [x] 2.4 Add optional post-send follow-up creation setting and behavior.
- [x] 2.5 Record follow-up activities.
- [x] 2.6 Add application tests for task creation, completion, dashboard query, and auto creation.

## 3. API

- [x] 3.1 Add REST endpoints for follow-up tasks.
- [x] 3.2 Add dashboard data endpoint if needed by Web.
- [x] 3.3 Update OpenAPI documentation.

## 4. Web

- [x] 4.1 Add follow-up list and creation UI.
- [x] 4.2 Add pending follow-ups to dashboard.
- [x] 4.3 Add follow-ups section to contact detail.
- [x] 4.4 Add complete action.

## 5. Verification

- [x] 5.1 Add EF and API integration tests.
- [x] 5.2 Run `dotnet test`.
- [x] 5.3 Update README with follow-up behavior.
