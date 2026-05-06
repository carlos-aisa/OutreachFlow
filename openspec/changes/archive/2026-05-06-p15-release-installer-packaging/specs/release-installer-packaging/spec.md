## ADDED Requirements

### Requirement: Installer generation in release workflow
The release workflow SHALL generate a Windows installer artifact for the application during release execution.

#### Scenario: Build installer on release run
- **WHEN** the manual release workflow is triggered with a valid change id and version
- **THEN** the workflow builds a versioned installer artifact as part of the release pipeline

### Requirement: Release asset publication
The release workflow SHALL publish the generated installer as an asset on the corresponding GitHub release.

#### Scenario: Publish installer asset
- **WHEN** installer generation succeeds
- **THEN** the installer file is attached to the release tag for that version

### Requirement: Packaging failure stops release
The release workflow SHALL fail if installer packaging or installer asset publication fails.

#### Scenario: Fail release on packaging error
- **WHEN** installer generation returns an error or expected installer output is missing
- **THEN** the workflow stops and reports a failed release job

### Requirement: Versioned installer naming
The generated installer SHALL follow a deterministic naming convention containing release version and target architecture.

#### Scenario: Installer file naming
- **WHEN** release version is 1.2.3 for x64
- **THEN** the generated installer name includes 1.2.3 and x64 in the filename
