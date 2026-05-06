## 1. Packaging Setup

- [x] 1.1 Choose and configure installer toolchain for Windows packaging in CI.
- [x] 1.2 Add installer configuration files and versioned output naming convention.
- [x] 1.3 Add release packaging script under scripts/release for reproducible installer generation.

## 2. Workflow Integration

- [x] 2.1 Update release-openspec-change workflow to include installer packaging step.
- [x] 2.2 Ensure workflow uses appropriate runner for packaging and preserves existing release validations.
- [x] 2.3 Add step to verify installer artifact exists and is non-empty before release publish.

## 3. Release Asset Publishing

- [x] 3.1 Attach installer artifact to GitHub release for the corresponding version tag.
- [x] 3.2 Ensure asset upload errors fail the workflow with clear logs.
- [x] 3.3 Validate that installer filename includes release version and architecture.

## 4. Verification and Documentation

- [x] 4.1 Add CI checks or smoke validation for installer packaging script.
- [x] 4.2 Run release workflow in a test release path and verify generated installer asset.
- [x] 4.3 Update README/release docs with installer artifact expectations and usage notes.
