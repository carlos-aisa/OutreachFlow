## 1. Domain and Contracts

- [x] 1.1 Add sender profile signature fields and signature format enum in Domain.
- [x] 1.2 Add domain validation rules for signature format and signature content consistency.
- [x] 1.3 Add/update application DTOs and requests for signature format/content.

## 2. Persistence and Migrations

- [x] 2.1 Extend EF Core mappings for sender profile signature columns.
- [x] 2.2 Create and verify migration for signature fields in relational database.
- [x] 2.3 Add persistence integration tests for HTML and RTF signature save/load.

## 3. Rendering and Draft Composition

- [x] 3.1 Update draft generation/template composition flow to append sender signature when present.
- [x] 3.2 Keep rendered body unchanged when sender profile has no signature.
- [x] 3.3 Add tests for signature append behavior and no-signature behavior.

## 4. API and Web

- [x] 4.1 Update sender profile API endpoints and models to accept and return signature format/content.
- [x] 4.2 Update OpenAPI specification for sender profile signature fields and validation responses.
- [x] 4.3 Add Web UI fields in sender profile forms for signature format selection and signature content.

## 5. Verification

- [x] 5.1 Add API integration tests for signature validation and persistence behavior.
- [x] 5.2 Run dotnet test and resolve any failing suites.
- [x] 5.3 Update user-facing documentation for sender signature behavior.
