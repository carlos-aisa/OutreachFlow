## Context

Once contacts, history, and sending exist, users need a practical way to plan next actions. Follow-up tasks should be simple, contact-centered, and optionally generated after sends.

## Goals / Non-Goals

**Goals:**

- Create, update, list, and complete follow-up tasks.
- Associate tasks with contacts and optionally organizations.
- Display pending tasks on dashboard and contact detail.
- Record activity when follow-ups are created or completed.

**Non-Goals:**

- Calendar sync.
- Recurring task engine.
- Notifications or background reminders.

## Decisions

- Keep follow-ups as first-class persisted entities rather than activity metadata.
- Use completion state and timestamps instead of deleting completed tasks.
- Configure automatic post-send follow-up creation as an optional application setting.
- Record follow-up activity through the contact activity mechanism.

## Risks / Trade-offs

- Automatic task creation can create noise. Mitigation: default to manual tasks or conservative opt-in configuration.
- Due date behavior can become timezone-sensitive. Mitigation: store dates/times explicitly and keep UI clear.

## Migration Plan

- Add follow-up task table and indexes by due date and contact.
- Add API/UI and activity integration.

## Open Questions

- Automatic task defaults should remain disabled unless explicitly configured.
