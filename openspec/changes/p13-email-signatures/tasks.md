## 1. Domain and Contracts

- [ ] 1.1 Add sender profile signature fields and signature format enum in Domain.
- [ ] 1.2 Add domain validation rules for signature format and signature content consistency.
- [ ] 1.3 Add/update application DTOs and requests for signature format/content.

## 2. Persistence and Migrations

- [ ] 2.1 Extend EF Core mappings for sender profile signature columns.
- [ ] 2.2 Create and verify migration for signature fields in relational database.
- [ ] 2.3 Add persistence integration tests for HTML and RTF signature save/load.

## 3. Rendering and Draft Composition

- [ ] 3.1 Update draft generation/template composition flow to append sender signature when present.
- [ ] 3.2 Keep rendered body unchanged when sender profile has no signature.
- [ ] 3.3 Add tests for signature append behavior and no-signature behavior.

## 4. API and Web

- [ ] 4.1 Update sender profile API endpoints and models to accept and return signature format/content.
- [ ] 4.2 Update OpenAPI specification for sender profile signature fields and validation responses.
- [ ] 4.3 Add Web UI fields in sender profile forms for signature format selection and signature content.

## 5. Verification

- [ ] 5.1 Add API integration tests for signature validation and persistence behavior.
- [ ] 5.2 Run dotnet test and resolve any failing suites.
- [ ] 5.3 Update user-facing documentation for sender signature behavior.
