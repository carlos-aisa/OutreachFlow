## ADDED Requirements

### Requirement: Installer generation in release workflow
The release workflow SHALL generate Windows installer artifacts using an MSI package and setup bootstrapper executable.

#### Scenario: Build setup and MSI artifacts on release run
- **WHEN** the manual release workflow is triggered with a valid change id and version
- **THEN** the workflow builds both `OutreachFlow-v<version>-win-x64-setup.exe` and `OutreachFlow-v<version>-win-x64.msi`

### Requirement: Release asset publication
The release workflow SHALL publish both installer artifacts on the corresponding GitHub release.

#### Scenario: Publish setup and MSI assets
- **WHEN** installer generation succeeds
- **THEN** setup and MSI files are attached to the release tag for that version

### Requirement: Installer behavior
The installer SHALL configure both OutreachFlow.Api and OutreachFlow.Web for local service execution and support uninstall/upgrade.

#### Scenario: Install configures both services
- **WHEN** a user completes the installer wizard
- **THEN** OutreachFlow.Api and OutreachFlow.Web services are registered and started
- **AND** runtime data paths are configured outside the install directory

#### Scenario: Uninstall removes service registrations
- **WHEN** the product is uninstalled
- **THEN** both services are stopped and unregistered
