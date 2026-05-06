## Context

The current release workflow validates OpenSpec state, builds, tests, tags, and creates a GitHub release, but it does not package an installer artifact for end-user installation. To support easier distribution, installer packaging must be integrated into the existing release pipeline with deterministic versioning and failure handling.

## Goals / Non-Goals

**Goals:**
- Generate a Windows installer artifact as part of release workflow execution.
- Publish the installer as an asset in the GitHub release for the same version tag.
- Ensure release pipeline fails if installer packaging fails.
- Keep installer naming deterministic and versioned.

**Non-Goals:**
- Multi-platform installer generation in this phase.
- Auto-update client mechanism.
- Replacing existing release validation and tagging behavior.

## Decisions

- Keep packaging inside the existing release-openspec-change workflow to preserve a single release path.
- Use a dedicated packaging script under scripts/release to isolate installer build logic from workflow YAML.
- Produce a versioned installer filename including product name, version, and architecture.
- Upload installer as a release asset after release creation, or include upload in release creation step.
- Add verification step to assert installer file exists and is non-empty before publishing.

## Risks / Trade-offs

- Installer toolchain may require Windows-specific runner, increasing workflow complexity -> Mitigation: split packaging job to windows-latest and keep validation/build/test on current runner.
- Packaging can increase release duration -> Mitigation: reuse Release build outputs and avoid duplicate compilation where possible.
- Asset upload failures can leave partially completed releases -> Mitigation: enforce atomic publish order and fail workflow with clear recovery instructions.
