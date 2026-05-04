## 1. Domain Model

- [ ] 1.1 Add `EmailDraft` domain model and status enum.
- [ ] 1.2 Add `EmailDraftAttachment` relationship model.
- [ ] 1.3 Add domain behavior for generated draft status and attachment assignment.
- [ ] 1.4 Add domain tests for draft creation and invalid attachment rules.

## 2. Persistence

- [ ] 2.1 Add EF Core mappings for drafts and draft attachments.
- [ ] 2.2 Add repository support for draft creation and listing.
- [ ] 2.3 Create and verify the EF Core migration.

## 3. Application

- [ ] 3.1 Add draft generation request/response DTOs.
- [ ] 3.2 Implement contact selection by filters and tags.
- [ ] 3.3 Implement `GenerateEmailDrafts` use case using the template renderer.
- [ ] 3.4 Store render diagnostics and skipped contact results.
- [ ] 3.5 Add application tests for multi-contact generation and error detection.

## 4. API

- [ ] 4.1 Add `POST /api/v1/drafts/generate`.
- [ ] 4.2 Add draft list and detail endpoints needed by the generation result.
- [ ] 4.3 Update OpenAPI documentation.

## 5. Web

- [ ] 5.1 Add recipient filter step to draft generation wizard.
- [ ] 5.2 Add template, sender, and attachment selection steps.
- [ ] 5.3 Add preview and generation result step.

## 6. Verification

- [ ] 6.1 Add EF integration tests for draft persistence.
- [ ] 6.2 Add API integration tests for draft generation.
- [ ] 6.3 Run `dotnet test`.
- [ ] 6.4 Update README or workflow documentation for draft generation.
