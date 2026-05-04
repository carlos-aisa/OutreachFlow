## Context

Sending records alone do not provide a full relationship timeline. Activity history collects relevant events in a contact-centered view while keeping the event model simple for MVP.

## Goals / Non-Goals

**Goals:**

- Record relevant contact activities.
- Query activities by contact in reverse chronological order.
- Display a contact timeline in Blazor.
- Keep metadata extensible through JSON while core fields remain queryable.

**Non-Goals:**

- Event sourcing.
- Full audit logging of every database field.
- Reply synchronization from external providers.

## Decisions

- `ContactActivity` is a persisted activity record, not a domain event store.
- Store `Type`, optional subject/body preview, optional metadata JSON, and occurrence timestamp.
- Record activity from application use cases after successful state changes.
- Keep metadata JSON provider-neutral and non-sensitive.

## Risks / Trade-offs

- Activity recording can be missed if use cases do not call it consistently. Mitigation: centralize activity creation helpers in Application and cover key workflows with tests.
- Metadata JSON can drift. Mitigation: keep required query fields as columns and use JSON only for contextual details.

## Migration Plan

- Add contact activity table and indexes by contact and occurred timestamp.
- Backfill is not required for MVP data.
- Update relevant use cases and tests.

## Open Questions

- None for MVP.
