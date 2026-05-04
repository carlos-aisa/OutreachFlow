## Why

OutreachFlow must keep humans in control before any email can be sent. This change adds draft review, editing, approval, and cancellation so generated drafts can be corrected before delivery.

## What Changes

- Add draft update/edit behavior for subject and body.
- Add approval and cancellation use cases.
- Validate drafts before approval.
- Add REST endpoints for listing, editing, approving, and cancelling drafts.
- Add Blazor draft list and detail screens.
- Add tests for approval rules and cancellation behavior.

## Capabilities

### New Capabilities

- `email-draft-review`: Human review, editing, approval, and cancellation of generated email drafts.

### Modified Capabilities

- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Depends on draft generation and template rendering diagnostics.
- Establishes the required approval gate before sending.
