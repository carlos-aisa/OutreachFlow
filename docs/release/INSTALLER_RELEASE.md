# Installer Release Packaging

`p15-release-installer-packaging` introduces deterministic installer package generation in the release workflow.

## Artifact Format

Current release packaging produces:

- `OutreachFlow-v<version>-win-x64-installer.zip`

Example:

- `OutreachFlow-v0.16.0-win-x64-installer.zip`

## Packaging Toolchain

- `dotnet publish` targeting `win-x64` (`--self-contained false`)
- PowerShell packaging script:
  - `scripts/release/Build-InstallerPackage.ps1`
- Artifact validation script:
  - `scripts/release/Validate-InstallerPackage.ps1`

## Local Packaging

Run from repository root:

```powershell
pwsh ./scripts/release/Build-InstallerPackage.ps1 -Version 0.16.0
pwsh ./scripts/release/Validate-InstallerPackage.ps1 -Version 0.16.0 -ArtifactPath ./artifacts/installer/OutreachFlow-v0.16.0-win-x64-installer.zip
```

## Release Workflow Integration

The manual release workflow (`.github/workflows/release-openspec-change.yml`) now:

1. builds the installer package,
2. validates file existence, filename convention, and non-empty size,
3. uploads the package as a workflow artifact,
4. creates the GitHub release,
5. uploads installer package to the release assets.

If packaging or asset upload fails, the release job fails.
