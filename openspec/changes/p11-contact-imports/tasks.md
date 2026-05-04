## 1. Domain and Persistence

- [ ] 1.1 Add `ImportJob` model and status enum if persisted as a domain concept.
- [ ] 1.2 Add EF Core mapping for import jobs.
- [ ] 1.3 Create and verify the EF Core migration.

## 2. Application

- [ ] 2.1 Add CSV import preview DTOs.
- [ ] 2.2 Add structured CSV parser service.
- [ ] 2.3 Add preview use case with row validation and duplicate detection.
- [ ] 2.4 Add commit use case with duplicate protection and tag assignment.
- [ ] 2.5 Add application tests for preview, validation, duplicates, and commit.

## 3. API

- [ ] 3.1 Add endpoint for CSV import preview.
- [ ] 3.2 Add endpoint for committing an import.
- [ ] 3.3 Add endpoint for listing import jobs if needed by UI.
- [ ] 3.4 Update OpenAPI documentation.

## 4. Web

- [ ] 4.1 Add CSV upload UI.
- [ ] 4.2 Add preview table with validation and duplicate indicators.
- [ ] 4.3 Add tag selection during import.
- [ ] 4.4 Add commit result summary.

## 5. Verification

- [ ] 5.1 Add integration tests for import endpoints.
- [ ] 5.2 Run `dotnet test`.
- [ ] 5.3 Update README with CSV format and import limitations.
