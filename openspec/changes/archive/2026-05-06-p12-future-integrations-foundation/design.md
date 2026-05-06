## Context

The roadmap includes several future integrations, but adding them before the MVP is stable would increase risk. This phase records extension boundaries and makes small preparatory changes only when they pay for themselves.

## Goals / Non-Goals

**Goals:**

- Document provider boundaries and constraints for future integrations.
- Keep current provider abstractions stable.
- Prepare configuration shape for optional future services.
- Preserve the rule that external contact systems are not modified automatically.

**Non-Goals:**

- Gmail API sender.
- Microsoft Graph sender.
- Google/Outlook contact sync.
- Background sending queue.
- PostgreSQL switch.
- Docker Compose or OpenTelemetry implementation unless explicitly approved as a later change.

## Decisions

- Treat this phase as a foundation and documentation change first.
- Add code only when a missing abstraction is already causing duplication or provider coupling.
- Keep future integrations opt-in and disabled by default.
- Prefer separate OpenSpec changes for each real integration when implementation starts.

## Risks / Trade-offs

- Over-preparing can become overengineering. Mitigation: avoid implementation unless it simplifies existing code.
- Future provider details may change. Mitigation: document boundaries, not detailed provider internals.

## Migration Plan

- No database migration is expected.
- Update architecture and roadmap documentation.
- Add focused tests only if configuration or code extension points change.

## Open Questions

- Each future integration should receive its own OpenSpec change before implementation.
