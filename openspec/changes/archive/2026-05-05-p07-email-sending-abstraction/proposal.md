## Why

OutreachFlow needs controlled sending without coupling the application to one email provider. This change adds the provider abstraction, fake sender, send use case, and send trace records.

## What Changes

- Add `IEmailSender`, `SendEmailCommand`, and `EmailSendResult` contracts.
- Add `FakeEmailSender` for development and tests.
- Add `EmailMessage` domain/persistence model.
- Add `SendApprovedDraft` use case.
- Enforce sending rules: approved only, no do-not-contact, valid email, no unresolved variables, active attachments, no duplicate sending.
- Add REST endpoint and UI action for sending approved drafts.
- Add tests for successful send, failure handling, and duplicate prevention.

## Capabilities

### New Capabilities

- `email-sending`: Controlled sending of approved drafts through an email sender abstraction.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Adds email message persistence and fake provider implementation.
- Depends on draft approval and contact management.
