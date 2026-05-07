## Context

OutreachFlow currently publishes a versioned zip artifact, but users requested a proper installer wizard that installs both API and Web processes, supports uninstall, and allows clean upgrades. A WiX-based MSI + bootstrapper setup provides this operational model without requiring manual file copy or script execution by end users.

## Goals / Non-Goals

**Goals:**
- Produce `OutreachFlow-v<version>-win-x64-setup.exe` and `OutreachFlow-v<version>-win-x64.msi`.
- Install API and Web as Windows services with deterministic local ports.
- Persist runtime data (SQLite DB and attachments) under ProgramData.
- Support uninstall and major-upgrade replacement behavior.
- Keep release workflow deterministic and auditable.

**Non-Goals:**
- MSI UI customization beyond standard WiX wizard pages.
- Automatic in-app update checks.
- Linux/macOS installers.

## Decisions

- Use WiX v4 (`WixToolset.Sdk`) for both MSI and bootstrapper projects.
- Publish API and Web as self-contained win-x64 outputs to avoid runtime prerequisite drift.
- Register services in installer custom actions via PowerShell scripts.
- Persist mutable runtime data under `C:\ProgramData\OutreachFlow`.
- Keep release workflow manual and tied to archived OpenSpec changes.

## Risks / Trade-offs

- Self-contained artifacts increase installer size.
  - Mitigation: deterministic packaging and explicit release assets.
- Service registration requires elevated installer execution.
  - Mitigation: MSI per-machine deployment and clear failure logs.
- Custom action scripts can fail on edge Windows configurations.
  - Mitigation: strict validation, explicit error handling, and idempotent service operations.
