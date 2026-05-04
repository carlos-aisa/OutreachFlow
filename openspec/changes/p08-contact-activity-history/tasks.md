## 1. Domain and Persistence

- [ ] 1.1 Add `ContactActivity` domain model and activity type enum.
- [ ] 1.2 Add EF Core mapping and indexes for contact activities.
- [ ] 1.3 Create and verify the EF Core migration.
- [ ] 1.4 Add domain tests for activity creation.

## 2. Application

- [ ] 2.1 Add activity DTOs and contact activity query use case.
- [ ] 2.2 Add activity recording helper/service in Application.
- [ ] 2.3 Record activity for contact create/update and status changes.
- [ ] 2.4 Record activity for draft creation and send success/failure.
- [ ] 2.5 Add application tests for activity recording.

## 3. API

- [ ] 3.1 Add `GET /api/v1/contacts/{id}/activities`.
- [ ] 3.2 Update OpenAPI documentation.
- [ ] 3.3 Add API integration tests for contact activity history.

## 4. Web

- [ ] 4.1 Add contact detail page if not already present.
- [ ] 4.2 Add activity timeline section to contact detail.

## 5. Verification

- [ ] 5.1 Add EF integration tests for ordering and filtering.
- [ ] 5.2 Run `dotnet test`.
- [ ] 5.3 Update README or architecture documentation for traceability behavior.
