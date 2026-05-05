## 1. Domain Model

- [x] 1.1 Add `EmailDraft` domain model and status enum.
- [x] 1.2 Add `EmailDraftAttachment` relationship model.
- [x] 1.3 Add domain behavior for generated draft status and attachment assignment.
- [x] 1.4 Add domain tests for draft creation and invalid attachment rules.

## 2. Persistence

- [x] 2.1 Add EF Core mappings for drafts and draft attachments.
- [x] 2.2 Add repository support for draft creation and listing.
- [x] 2.3 Create and verify the EF Core migration.

## 3. Application

- [x] 3.1 Add draft generation request/response DTOs.
- [x] 3.2 Implement contact selection by filters and tags.
- [x] 3.3 Implement `GenerateEmailDrafts` use case using the template renderer.
- [x] 3.4 Store render diagnostics and skipped contact results.
- [x] 3.5 Add application tests for multi-contact generation and error detection.

## 4. API

- [x] 4.1 Add `POST /api/v1/drafts/generate`.
- [x] 4.2 Add draft list and detail endpoints needed by the generation result.
- [x] 4.3 Update OpenAPI documentation.

## 5. Web

- [x] 5.1 Add recipient filter step to draft generation wizard.
- [x] 5.2 Add template, sender, and attachment selection steps.
- [x] 5.3 Add preview and generation result step.

## 6. Verification

- [x] 6.1 Add EF integration tests for draft persistence.
- [x] 6.2 Add API integration tests for draft generation.
- [x] 6.3 Run `dotnet test`.
- [x] 6.4 Update README or workflow documentation for draft generation.
