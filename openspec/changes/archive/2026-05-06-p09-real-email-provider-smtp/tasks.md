## 1. Configuration

- [x] 1.1 Add SMTP options model.
- [x] 1.2 Add provider selection configuration.
- [x] 1.3 Add validation for required SMTP settings when SMTP is selected.

## 2. Infrastructure

- [x] 2.1 Implement `SmtpEmailSender` behind `IEmailSender`.
- [x] 2.2 Map sender profile, recipient, subject, body, and attachments to SMTP messages.
- [x] 2.3 Wrap infrastructure errors with useful context and no secret logging.
- [x] 2.4 Register SMTP sender conditionally through DI.

## 3. Tests

- [x] 3.1 Add tests for SMTP options validation.
- [x] 3.2 Add tests for provider selection.
- [x] 3.3 Add tests for failure mapping without real network calls.
- [x] 3.4 Verify existing sending integration tests still use `FakeEmailSender`.

## 4. Documentation

- [x] 4.1 Document SMTP configuration with placeholder values.
- [x] 4.2 Document user secrets/environment variable setup.
- [x] 4.3 Update README with SMTP limitations.

## 5. Verification

- [x] 5.1 Run `dotnet test`.
- [x] 5.2 Confirm no secrets are committed.
