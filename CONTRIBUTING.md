# Contributing to OutreachFlow

Thank you for contributing.

## Workflow

- Create a branch from `main` using `change/<change-id-or-short-description>`.
- Keep one OpenSpec change per branch and per pull request.
- Open a pull request to `main` using the provided PR template.
- Ensure CI is green before requesting review.

## Development Checklist

- Keep business logic in Domain/Application, not in UI or transport layers.
- Add or update tests for any behavior change.
- Update documentation when behavior, API contracts, data models, or configuration changes.
- If API contracts change, update `docs/api/openapi.v1.yaml`.

## Local Verification

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release
```

## Pull Request Expectations

- Clear summary of the change and motivation
- Linked OpenSpec change id
- Test evidence
- Notes for reviewers (edge cases, tradeoffs, known limitations)

## Commit Convention

Use Conventional Commits, for example:

- `feat: add follow-up completion endpoint`
- `fix: block send when unresolved variables remain`
- `docs: update release workflow section`

## Code of Conduct

Be respectful, constructive, and specific in technical feedback.
