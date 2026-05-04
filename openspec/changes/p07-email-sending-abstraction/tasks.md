## 1. Contracts

- [ ] 1.1 Add `IEmailSender` application interface.
- [ ] 1.2 Add `SendEmailCommand`, attachment payload, sender payload, and metadata models.
- [ ] 1.3 Add `EmailSendResult` model.

## 2. Domain and Persistence

- [ ] 2.1 Add `EmailMessage` domain model and status enum.
- [ ] 2.2 Add send-related draft state transition behavior.
- [ ] 2.3 Add EF Core mapping for email messages.
- [ ] 2.4 Create and verify the EF Core migration.

## 3. Infrastructure

- [ ] 3.1 Implement `FakeEmailSender`.
- [ ] 3.2 Add provider configuration for fake sender.
- [ ] 3.3 Register sender implementation in DI.

## 4. Application

- [ ] 4.1 Implement `SendApprovedDraft` use case.
- [ ] 4.2 Validate approved state, recipient eligibility, unresolved variables, active attachments, and duplicate send rules.
- [ ] 4.3 Persist email message results and update contact last contacted timestamp.
- [ ] 4.4 Add application tests for success, failure, blocked sends, and duplicate prevention.

## 5. API and Web

- [ ] 5.1 Add `POST /api/v1/drafts/{id}/send`.
- [ ] 5.2 Update OpenAPI documentation.
- [ ] 5.3 Add send action to approved draft UI.

## 6. Verification

- [ ] 6.1 Add integration tests for simulated send and duplicate prevention.
- [ ] 6.2 Run `dotnet test`.
- [ ] 6.3 Update README with fake sender behavior.
