## Why

OutreachFlow needs to generate personalized drafts for selected contacts while keeping review and safety checks visible before any email is sent. This change connects contacts, templates, sender profiles, attachments, and the renderer into a draft generation workflow.

## What Changes

- Add email draft and draft attachment domain and persistence models.
- Add draft generation use case for multiple contacts.
- Select contacts by filters/tags/status and generate one draft per eligible contact.
- Apply template rendering and set `NeedsReview` when variables are missing or unknown.
- Copy default template attachments and allow optional attachment selection.
- Add REST endpoints and Blazor wizard for draft generation.
- Add tests for successful generation, render errors, ineligible contacts, and attachment validation.

## Capabilities

### New Capabilities

- `email-draft-generation`: Personalized email draft generation from contacts, templates, sender profiles, and attachments.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Depends on contacts, templates, template rendering, sender profiles, and attachments.
- Adds database tables for email drafts and draft attachments.
