## Why

The current release process publishes source-based outputs but does not produce an end-user installer artifact. Adding installer generation to release makes deployment easier and more consistent for users who need a packaged application setup.

## What Changes

- Add release pipeline behavior to build a Windows installer for OutreachFlow.
- Attach installer artifact to the GitHub Release created by the release workflow.
- Add versioned installer naming conventions aligned with release version.
- Add validation steps to ensure installer generation fails the release if packaging fails.
- Add documentation for installer usage and release artifact expectations.

## Capabilities

### New Capabilities
- release-installer-packaging: Generate and publish an installer artifact during release workflow execution.

### Modified Capabilities
- None.

## Impact

- Affects GitHub Actions release workflow and release scripts.
- Introduces packaging toolchain and installer build configuration.
- Updates release documentation and verification steps.
