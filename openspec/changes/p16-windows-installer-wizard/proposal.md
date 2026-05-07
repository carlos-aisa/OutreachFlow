## Why

The current release artifact is a zip package, which is not an installer experience and does not provide guided install/uninstall flows for both OutreachFlow services.

## What Changes

- Replace zip packaging with a Windows installer wizard flow (`setup.exe` + `.msi`).
- Install and configure both OutreachFlow.Api and OutreachFlow.Web as Windows services.
- Add uninstall/update support through MSI major upgrade behavior.
- Keep release automation aligned with OpenSpec release validation and SemVer.

## Capabilities

### Modified Capabilities
- release-installer-packaging: Move from zip-only packaging to MSI + setup wizard release assets.

## Impact

- Adds WiX installer projects and installer PowerShell scripts.
- Updates API and Web hosting startup for Windows service execution.
- Updates release workflow and installer documentation.
