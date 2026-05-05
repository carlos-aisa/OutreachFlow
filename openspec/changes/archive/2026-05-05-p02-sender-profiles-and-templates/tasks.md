## 1. Domain Model

- [x] 1.1 Add `SenderProfile` domain model with default profile behavior.
- [x] 1.2 Add `EmailTemplate` domain model with active state behavior.
- [x] 1.3 Add tests for sender profile creation, default behavior, and template creation.

## 2. Persistence

- [x] 2.1 Add EF Core mappings for sender profiles and email templates.
- [x] 2.2 Add indexes needed for default sender lookup and template listing.
- [x] 2.3 Add repositories for sender profiles and email templates.
- [x] 2.4 Create and verify the EF Core migration.

## 3. Application

- [x] 3.1 Add DTOs and use cases for sender profile CRUD.
- [x] 3.2 Add DTOs and use cases for email template CRUD.
- [x] 3.3 Add centralized supported template variable contract.
- [x] 3.4 Add application tests for default profile behavior and template active state.

## 4. API

- [x] 4.1 Add REST endpoints for sender profiles.
- [x] 4.2 Add REST endpoints for email templates.
- [x] 4.3 Add endpoint for supported template variables.
- [x] 4.4 Update OpenAPI documentation.

## 5. Web

- [x] 5.1 Add Blazor sender profile list and edit pages.
- [x] 5.2 Add Blazor template list and edit pages.
- [x] 5.3 Display supported variables in the template editor.

## 6. Verification

- [x] 6.1 Add EF integration tests for sender profile and template persistence.
- [x] 6.2 Add API integration tests for sender profile and template endpoints.
- [x] 6.3 Run `dotnet test`.
- [x] 6.4 Update README or architecture documentation if behavior changes.
