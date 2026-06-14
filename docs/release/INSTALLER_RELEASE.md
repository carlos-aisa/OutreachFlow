# Installer Release Packaging

`p16-windows-installer-wizard` replaces zip-only distribution with a real Windows installer wizard flow.

## Artifact Format

Current release packaging produces two assets:

- `OutreachFlow-v<version>-win-x64-setup.exe`
- `OutreachFlow-v<version>-win-x64.msi`

Example for version `0.17.0`:

- `OutreachFlow-v0.17.0-win-x64-setup.exe`
- `OutreachFlow-v0.17.0-win-x64.msi`

## Packaging Toolchain

- WiX v4 installer projects under `tools/installer/windows/`
- PowerShell build script:
  - `tools/installer/windows/build-installer.ps1`
- Release artifact validation script:
  - `scripts/release/Validate-WindowsInstallerArtifacts.ps1`

## Local Packaging

Run from repository root:

```powershell
pwsh ./tools/installer/windows/build-installer.ps1 -Version 0.17.0 -Configuration Release
pwsh ./scripts/release/Validate-WindowsInstallerArtifacts.ps1 -Version 0.17.0 `
  -SetupPath ./artifacts/installer/OutreachFlow-v0.17.0-win-x64-setup.exe `
  -MsiPath ./artifacts/installer/OutreachFlow-v0.17.0-win-x64.msi
```

`build-installer.ps1` writes build progress to the host stream and reserves standard output for its final JSON summary. This allows CI steps to either invoke it directly or safely capture the JSON result in a PowerShell variable before calling `ConvertFrom-Json`.

## Installation Behavior

The MSI custom actions:

- configure runtime data under `C:\ProgramData\OutreachFlow`,
- configure API and Web appsettings for local service mode,
- register and start Windows services:
  - `OutreachFlow.Api`
  - `OutreachFlow.Web`

Uninstall removes service registrations and can optionally remove runtime data when requested.

## Release Workflow Integration

The manual release workflow (`.github/workflows/release-openspec-change.yml`) now:

1. validates OpenSpec release constraints,
2. restores/builds/tests the solution,
3. builds setup + MSI assets,
4. validates naming and non-empty artifact output,
5. uploads both artifacts to workflow artifacts,
6. creates the GitHub release,
7. uploads setup + MSI to release assets.

If installer build or asset upload fails, the release job fails.
