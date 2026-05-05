## Why

OutreachFlow needs reusable email signatures so outbound messages keep a consistent sender identity without manual copy and paste. This should support rich formatting because users commonly maintain signatures in HTML or RTF.

## What Changes

- Add sender profile signature content that can be stored in HTML or RTF format.
- Add signature rendering behavior that appends the selected sender signature at the end of generated email body content.
- Add API and UI support to create, update, preview, and validate signature format and content.
- Add tests for signature persistence, validation, and rendering composition.

## Capabilities

### New Capabilities
- email-signature-management: Store and manage sender signatures in HTML or RTF, and append them to generated emails.

### Modified Capabilities
- None.

## Impact

- Affects Domain, Application, Infrastructure, Api, Web, and tests.
- Touches sender profile workflows and template rendering/draft composition paths.
- May require schema changes for signature content and declared format.
