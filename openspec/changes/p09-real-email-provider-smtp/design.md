## Context

The fake sender validates application flow but cannot deliver real emails. SMTP provides a real provider option without coupling core layers to one external platform.

## Goals / Non-Goals

**Goals:**

- Implement SMTP sender behind `IEmailSender`.
- Configure host, port, TLS, username, password, and default provider selection through environment-based configuration.
- Send subject, body, recipient, sender, and attachments.
- Document local setup and secret handling.

**Non-Goals:**

- Gmail API or Microsoft Graph providers.
- Reading replies or syncing mailbox state.
- Running SMTP integration tests against a real external provider.

## Decisions

- Keep SMTP implementation in Infrastructure only.
- Use framework SMTP APIs or an approved SMTP library only if required by implementation constraints.
- Read secrets from configuration providers, user secrets, or environment variables.
- Tests must avoid real network calls and continue to rely on fake sender for full send workflows.

## Risks / Trade-offs

- SMTP deliverability varies by provider. Mitigation: document provider-specific setup as local configuration, not application logic.
- Secrets can leak if misconfigured. Mitigation: keep examples placeholder-only and ensure `.gitignore` excludes local secret files.

## Migration Plan

- No schema migration is expected.
- Add configuration classes and provider registration.
- Update documentation and tests.

## Open Questions

- The first version can use plain text body only; HTML support can be added later if needed.
