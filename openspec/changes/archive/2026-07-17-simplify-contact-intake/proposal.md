## Why

Creating a contact currently exposes separate organization and contact management screens, which makes ordinary data entry feel like a technical prerequisite chain. The primary user needs a guided, contact-first way to record people and their organizations without leaving the task.

## What Changes

- Add a contact-first intake flow that can create a new organization or associate an existing one while creating a contact.
- Make organization association clearly optional and provide a clear next action after a contact is saved.
- Reorganize the contact entry experience around everyday data entry rather than filters and entity administration.

## Capabilities

### New Capabilities
- `guided-contact-intake`: Contact-first creation flow with in-context organization creation or association and contextual next actions.

### Modified Capabilities
- `contact-management`: Clarify the user-facing contact creation flow while preserving optional organization association.
- `web-ui-appearance`: Extend navigation and page-hierarchy requirements for task-oriented contact entry.

## Impact

- Affects the Blazor contacts and organizations surfaces, localization resources, and contact/organization API client usage.
- May add an application/API operation that creates a contact and a new organization atomically.
- Requires web component, application/API, and relational integration tests; API contract changes require OpenAPI updates.
