## 1. Contracts

- [x] 1.1 Add `IEmailSender` application interface.
- [x] 1.2 Add `SendEmailCommand`, attachment payload, sender payload, and metadata models.
- [x] 1.3 Add `EmailSendResult` model.

## 2. Domain and Persistence

- [x] 2.1 Add `EmailMessage` domain model and status enum.
- [x] 2.2 Add send-related draft state transition behavior.
- [x] 2.3 Add EF Core mapping for email messages.
- [x] 2.4 Create and verify the EF Core migration.

## 3. Infrastructure

- [x] 3.1 Implement `FakeEmailSender`.
- [x] 3.2 Add provider configuration for fake sender.
- [x] 3.3 Register sender implementation in DI.

## 4. Application

- [x] 4.1 Implement `SendApprovedDraft` use case.
- [x] 4.2 Validate approved state, recipient eligibility, unresolved variables, active attachments, and duplicate send rules.
- [x] 4.3 Persist email message results and update contact last contacted timestamp.
- [x] 4.4 Add application tests for success, failure, blocked sends, and duplicate prevention.

## 5. API and Web

- [x] 5.1 Add `POST /api/v1/drafts/{id}/send`.
- [x] 5.2 Update OpenAPI documentation.
- [x] 5.3 Add send action to approved draft UI.

## 6. Verification

- [x] 6.1 Add integration tests for simulated send and duplicate prevention.
- [x] 6.2 Run `dotnet test`.
- [x] 6.3 Update README with fake sender behavior.
