## Why

OutreachFlow needs traceability for every meaningful contact interaction. This change adds contact activity history so users can understand what happened with each contact over time.

## What Changes

- Add `ContactActivity` domain and persistence model.
- Add activity recording for key events: contact creation/update, draft creation, send success/failure, status change, notes, and future follow-up events.
- Add contact activity endpoint.
- Add activity timeline to contact detail UI.
- Add tests for activity creation and ordering.

## Capabilities

### New Capabilities

- `contact-activity-history`: Timeline of contact-related activity for traceable relationship management.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Integrates with existing contact, draft, and sending workflows.
- Adds database table and query endpoints for activities.
