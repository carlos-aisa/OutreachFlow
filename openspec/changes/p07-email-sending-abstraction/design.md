## Context

Drafts can be approved after Phase 6, but sending must remain controlled and provider-agnostic. The MVP starts with a fake sender to avoid external dependencies in development and tests.

## Goals / Non-Goals

**Goals:**

- Send only approved drafts.
- Abstract provider-specific sending behind `IEmailSender`.
- Persist attempted and successful sends as `EmailMessage`.
- Update draft sent/failure state and contact last contacted date.
- Prevent sending the same draft twice.

**Non-Goals:**

- SMTP, Gmail, or Microsoft Graph real providers.
- Background queues, retry policies, or rate limiting.
- Reply tracking.

## Decisions

- Define sender contracts in Application and implementations in Infrastructure.
- Persist `EmailMessage` for every send attempt that reaches the sending use case.
- Use `FakeEmailSender` as the default development/test provider.
- Keep duplicate-send detection on draft id and equivalent recent email rules in the application use case.

## Risks / Trade-offs

- Equivalent email detection can be fuzzy. Mitigation: MVP uses draft id and contact/template/subject recency checks with explicit override reserved for a later configurable rule.
- Fake sending can mask provider-specific errors. Mitigation: provider-specific behavior is introduced and tested separately in Phase 9.

## Migration Plan

- Add email message table.
- Add fake provider registration.
- Add sending endpoint and tests.

## Open Questions

- Equivalent recent email window can start as a configurable setting with a conservative default.
