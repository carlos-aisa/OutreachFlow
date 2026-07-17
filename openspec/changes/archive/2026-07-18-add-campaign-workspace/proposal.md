## Why

Periodic outreach is currently spread across draft generation, drafts, templates, sender profiles, and follow-ups. A campaign workspace is needed to organize a recurring outreach initiative around its purpose, message, audience, and progress without forcing a fixed creation order.

## What Changes

- Add persistent campaigns that users can save in a partially completed state.
- Provide a flexible campaign workspace for message, recipients, sender profile, attachments, and follow-up configuration in any order.
- Use completion guidance rather than a mandatory wizard; validate prerequisites only when generating drafts or sending.
- Reorganize primary navigation and dashboard work queues around contacts, groups, campaigns, review, and follow-up work.

## Capabilities

### New Capabilities
- `campaign-management`: Persistent campaigns and a non-linear campaign workspace for periodic outreach preparation.

### Modified Capabilities
- `email-draft-generation`: Allow campaign-owned draft generation in addition to the current standalone wizard.
- `web-ui-appearance`: Define task-oriented navigation and campaign readiness guidance.

## Impact

- Adds campaign domain, persistence, application/API, OpenAPI, Blazor workspace, dashboard, navigation, and localization work.
- Depends functionally on `contact-group-management` when groups are selected as a campaign audience.
- Requires application, API, persistence, and web component tests.
