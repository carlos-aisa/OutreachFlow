## MODIFIED Requirements

### Requirement: Installer behavior
The installer SHALL configure both OutreachFlow.Api and OutreachFlow.Web for local service execution, support uninstall and upgrade, and place runtime data outside the install directory in a stable layout that backup and restore workflows can target.

#### Scenario: Install configures both services
- **WHEN** a user completes the installer wizard
- **THEN** OutreachFlow.Api and OutreachFlow.Web services are registered and started
- **AND** runtime data paths are configured outside the install directory

#### Scenario: Upgrade preserves runtime data layout
- **WHEN** a user upgrades an existing installation
- **THEN** the configured runtime data root for the database and attachments remains in the expected external runtime location

#### Scenario: Uninstall removes service registrations
- **WHEN** the product is uninstalled
- **THEN** both services are stopped and unregistered
