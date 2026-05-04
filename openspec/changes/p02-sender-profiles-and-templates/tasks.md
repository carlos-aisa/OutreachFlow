## 1. Domain Model

- [ ] 1.1 Add `SenderProfile` domain model with default profile behavior.
- [ ] 1.2 Add `EmailTemplate` domain model with active state behavior.
- [ ] 1.3 Add tests for sender profile creation, default behavior, and template creation.

## 2. Persistence

- [ ] 2.1 Add EF Core mappings for sender profiles and email templates.
- [ ] 2.2 Add indexes needed for default sender lookup and template listing.
- [ ] 2.3 Add repositories for sender profiles and email templates.
- [ ] 2.4 Create and verify the EF Core migration.

## 3. Application

- [ ] 3.1 Add DTOs and use cases for sender profile CRUD.
- [ ] 3.2 Add DTOs and use cases for email template CRUD.
- [ ] 3.3 Add centralized supported template variable contract.
- [ ] 3.4 Add application tests for default profile behavior and template active state.

## 4. API

- [ ] 4.1 Add REST endpoints for sender profiles.
- [ ] 4.2 Add REST endpoints for email templates.
- [ ] 4.3 Add endpoint for supported template variables.
- [ ] 4.4 Update OpenAPI documentation.

## 5. Web

- [ ] 5.1 Add Blazor sender profile list and edit pages.
- [ ] 5.2 Add Blazor template list and edit pages.
- [ ] 5.3 Display supported variables in the template editor.

## 6. Verification

- [ ] 6.1 Add EF integration tests for sender profile and template persistence.
- [ ] 6.2 Add API integration tests for sender profile and template endpoints.
- [ ] 6.3 Run `dotnet test`.
- [ ] 6.4 Update README or architecture documentation if behavior changes.
