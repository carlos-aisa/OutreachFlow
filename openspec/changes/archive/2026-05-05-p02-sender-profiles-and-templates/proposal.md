## Why

OutreachFlow must keep sender identity and email content configurable instead of hardcoded. This change adds sender profiles and reusable templates as the foundation for controlled personalization.

## What Changes

- Add sender profile domain and persistence model.
- Add email template domain and persistence model.
- Add CRUD use cases and REST endpoints for sender profiles and templates.
- Add Blazor screens for managing sender profiles and templates.
- Add a centralized list of supported template variables for UI help and future validation.
- Add tests for sender profile defaults, template creation, active state, and API behavior.

## Capabilities

### New Capabilities

- `sender-profile-management`: Configurable sender identities used by draft and email workflows.
- `email-template-management`: Reusable email subject/body templates with variable guidance.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Adds database tables and OpenAPI contracts for sender profiles and templates.
- Establishes data-driven sender identity and reusable message content.
