## 1. Configuration

- [ ] 1.1 Add SMTP options model.
- [ ] 1.2 Add provider selection configuration.
- [ ] 1.3 Add validation for required SMTP settings when SMTP is selected.

## 2. Infrastructure

- [ ] 2.1 Implement `SmtpEmailSender` behind `IEmailSender`.
- [ ] 2.2 Map sender profile, recipient, subject, body, and attachments to SMTP messages.
- [ ] 2.3 Wrap infrastructure errors with useful context and no secret logging.
- [ ] 2.4 Register SMTP sender conditionally through DI.

## 3. Tests

- [ ] 3.1 Add tests for SMTP options validation.
- [ ] 3.2 Add tests for provider selection.
- [ ] 3.3 Add tests for failure mapping without real network calls.
- [ ] 3.4 Verify existing sending integration tests still use `FakeEmailSender`.

## 4. Documentation

- [ ] 4.1 Document SMTP configuration with placeholder values.
- [ ] 4.2 Document user secrets/environment variable setup.
- [ ] 4.3 Update README with SMTP limitations.

## 5. Verification

- [ ] 5.1 Run `dotnet test`.
- [ ] 5.2 Confirm no secrets are committed.
