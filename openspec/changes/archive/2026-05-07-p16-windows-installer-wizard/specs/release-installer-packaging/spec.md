## MODIFIED Requirements

### Requirement: Installer generation in release workflow
The release workflow SHALL generate Windows installer artifacts for the application during release execution.

#### Scenario: Build setup and MSI artifacts on release run
- **WHEN** the manual release workflow is triggered with a valid change id and version
- **THEN** the workflow builds both `OutreachFlow-v<version>-win-x64-setup.exe` and `OutreachFlow-v<version>-win-x64.msi`

### Requirement: Release asset publication
The release workflow SHALL publish all generated installer artifacts as assets on the corresponding GitHub release.

#### Scenario: Publish setup and MSI assets
- **WHEN** installer generation succeeds
- **THEN** setup and MSI files are attached to the release tag for that version

### Requirement: Packaging failure stops release
The release workflow SHALL fail if installer packaging or installer asset publication fails.

#### Scenario: Fail release on packaging error
- **WHEN** installer generation returns an error or expected installer outputs are missing
- **THEN** the workflow stops and reports a failed release job

### Requirement: Versioned installer naming
The generated installer artifacts SHALL follow deterministic naming conventions containing release version and target architecture.

#### Scenario: Installer file naming
- **WHEN** release version is 1.2.3 for x64
- **THEN** generated artifact names include 1.2.3 and x64 in both setup and MSI filenames

## ADDED Requirements

### Requirement: Installer behavior
The installer SHALL configure both OutreachFlow.Api and OutreachFlow.Web for local service execution and support uninstall/upgrade.

#### Scenario: Install configures both services
- **WHEN** a user completes the installer wizard
- **THEN** OutreachFlow.Api and OutreachFlow.Web services are registered and started
- **AND** runtime data paths are configured outside the install directory

#### Scenario: Uninstall removes service registrations
- **WHEN** the product is uninstalled
- **THEN** both services are stopped and unregistered
