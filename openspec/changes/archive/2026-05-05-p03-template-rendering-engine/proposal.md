## Why

Templates are only useful when OutreachFlow can safely render them for a specific contact, organization, and sender. This change adds a small, testable rendering engine with explicit variable validation.

## What Changes

- Add `ITemplateRenderer`, `TemplateContext`, and `RenderedEmail` application contracts.
- Render supported `{{...}}` variables in subject and body.
- Detect unknown variables and known variables with missing values.
- Return structured render results with missing variables, unknown variables, and error state.
- Add exhaustive tests for rendering, missing values, unknown variables, and unresolved content.

## Capabilities

### New Capabilities

- `template-rendering`: Safe rendering and validation of email templates using centralized variables.

### Modified Capabilities

- None.

## Impact

- Affects Application and tests primarily.
- Later draft generation and sending phases depend on this capability.
- May add small API/UI support for previewing template render results if needed by the implementation.
