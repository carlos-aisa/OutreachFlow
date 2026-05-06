## Why

After fake sending is stable, OutreachFlow should support real local email sending through a simple provider. SMTP is the smallest practical real provider while preserving the provider abstraction.

## What Changes

- Add SMTP provider configuration with secure secret handling.
- Implement `SmtpEmailSender` in Infrastructure.
- Support attachments and sender profile values in SMTP send command mapping.
- Keep tests on fake sender and add unit tests for SMTP command/configuration behavior without contacting a real server.
- Update documentation for local SMTP configuration.

## Capabilities

### New Capabilities

- `smtp-email-provider`: Real SMTP email sending provider for local/manual configuration.

### Modified Capabilities

- None.

## Impact

- Affects Infrastructure, Application configuration, Api/Web setup, docs, and tests.
- Introduces SMTP-related dependencies only in Infrastructure.
- Requires secure configuration guidance and no committed secrets.
