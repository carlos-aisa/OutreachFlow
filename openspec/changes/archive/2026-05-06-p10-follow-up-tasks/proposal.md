## Why

OutreachFlow should help users remember the next relationship step after contact or email activity. Follow-up tasks make the CRM useful beyond one-off sending.

## What Changes

- Add follow-up task domain and persistence model.
- Add CRUD and completion use cases for manual follow-up tasks.
- Optionally create follow-up tasks after sending when configured.
- Show pending follow-ups on dashboard and contact detail.
- Add REST endpoints, Blazor screens, and tests.

## Capabilities

### New Capabilities

- `follow-up-tasks`: Manual and configurable follow-up task management for contacts.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Adds database table and dashboard/contact detail UI behavior.
- Integrates with contact and sending workflows.
