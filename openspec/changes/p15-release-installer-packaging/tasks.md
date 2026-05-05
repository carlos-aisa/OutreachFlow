## 1. Packaging Setup

- [ ] 1.1 Choose and configure installer toolchain for Windows packaging in CI.
- [ ] 1.2 Add installer configuration files and versioned output naming convention.
- [ ] 1.3 Add release packaging script under scripts/release for reproducible installer generation.

## 2. Workflow Integration

- [ ] 2.1 Update release-openspec-change workflow to include installer packaging step.
- [ ] 2.2 Ensure workflow uses appropriate runner for packaging and preserves existing release validations.
- [ ] 2.3 Add step to verify installer artifact exists and is non-empty before release publish.

## 3. Release Asset Publishing

- [ ] 3.1 Attach installer artifact to GitHub release for the corresponding version tag.
- [ ] 3.2 Ensure asset upload errors fail the workflow with clear logs.
- [ ] 3.3 Validate that installer filename includes release version and architecture.

## 4. Verification and Documentation

- [ ] 4.1 Add CI checks or smoke validation for installer packaging script.
- [ ] 4.2 Run release workflow in a test release path and verify generated installer asset.
- [ ] 4.3 Update README/release docs with installer artifact expectations and usage notes.
