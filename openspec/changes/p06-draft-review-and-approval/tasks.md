## 1. Domain Rules

- [ ] 1.1 Add draft state transitions for edit, approve, and cancel.
- [ ] 1.2 Add approval validation for unresolved variables and render errors.
- [ ] 1.3 Add domain tests for edit, approve, approval rejection, and cancellation.

## 2. Application

- [ ] 2.1 Add draft list/detail query DTOs.
- [ ] 2.2 Add update draft use case.
- [ ] 2.3 Add approve draft use case.
- [ ] 2.4 Add cancel draft use case.
- [ ] 2.5 Add application tests for approval and cancellation flows.

## 3. Persistence

- [ ] 3.1 Extend draft mappings if additional review metadata is required.
- [ ] 3.2 Create and verify migration if schema changes are needed.

## 4. API

- [ ] 4.1 Add `GET /api/v1/drafts`.
- [ ] 4.2 Add `GET /api/v1/drafts/{id}`.
- [ ] 4.3 Add `PUT /api/v1/drafts/{id}`.
- [ ] 4.4 Add `POST /api/v1/drafts/{id}/approve`.
- [ ] 4.5 Add `POST /api/v1/drafts/{id}/cancel`.
- [ ] 4.6 Update OpenAPI documentation.

## 5. Web

- [ ] 5.1 Add draft list page with status filters.
- [ ] 5.2 Add draft detail page with subject/body editing.
- [ ] 5.3 Add approve and cancel actions with validation feedback.

## 6. Verification

- [ ] 6.1 Add API integration tests for list, edit, approve, and cancel endpoints.
- [ ] 6.2 Run `dotnet test`.
- [ ] 6.3 Update workflow documentation for draft review.
