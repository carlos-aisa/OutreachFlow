## Why

OutreachFlow releases are currently created through a manually dispatched workflow that requires an explicit OpenSpec change id and version. The project may benefit from a safer, less manual release policy that calculates semantic versions after pull requests are merged, but the correct policy has not yet been decided.

## Exploration Scope

This change is intentionally parked for later exploration. It MUST NOT be implemented until the release policy has been agreed.

Questions to resolve:

- Whether every merge to `main` creates a release, or only selected pull requests do.
- Whether semantic version increments are declared by PR labels, conventional commit validation, or another explicit mechanism.
- Which merges build and publish Windows installer artifacts.
- How automated release notes, `CHANGELOG.md`, OpenSpec archival, tags, and GitHub Releases remain consistent.
- How the workflow prevents concurrent releases and handles rollback or failed publishing.

## Initial Direction

Any future design should retain the current safeguards: releases run only from `main`, validate tests and OpenSpec state, publish reproducible installer artifacts, and avoid publishing automatically merely because a contact is newly eligible for outreach.
