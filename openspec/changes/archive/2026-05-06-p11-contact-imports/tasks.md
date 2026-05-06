## 1. Domain and Persistence

- [x] 1.1 Add `ImportJob` model and status enum if persisted as a domain concept.
- [x] 1.2 Add EF Core mapping for import jobs.
- [x] 1.3 Create and verify the EF Core migration.

## 2. Application

- [x] 2.1 Add CSV import preview DTOs.
- [x] 2.2 Add structured CSV parser service.
- [x] 2.3 Add preview use case with row validation and duplicate detection.
- [x] 2.4 Add commit use case with duplicate protection and tag assignment.
- [x] 2.5 Add application tests for preview, validation, duplicates, and commit.

## 3. API

- [x] 3.1 Add endpoint for CSV import preview.
- [x] 3.2 Add endpoint for committing an import.
- [x] 3.3 Add endpoint for listing import jobs if needed by UI.
- [x] 3.4 Update OpenAPI documentation.

## 4. Web

- [x] 4.1 Add CSV upload UI.
- [x] 4.2 Add preview table with validation and duplicate indicators.
- [x] 4.3 Add tag selection during import.
- [x] 4.4 Add commit result summary.

## 5. Verification

- [x] 5.1 Add integration tests for import endpoints.
- [x] 5.2 Run `dotnet test`.
- [x] 5.3 Update README with CSV format and import limitations.
