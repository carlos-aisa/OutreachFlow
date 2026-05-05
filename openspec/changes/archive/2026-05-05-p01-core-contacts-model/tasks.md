## 1. Domain Model

- [x] 1.1 Add `Organization`, `Contact`, `Tag`, and `ContactTag` domain model types.
- [x] 1.2 Add contact status enum and domain validation for required fields.
- [x] 1.3 Add domain behavior for do-not-contact, status updates, and tag assignment/removal.
- [x] 1.4 Add domain tests for contact creation, tag assignment, status changes, and do-not-contact marking.

## 2. Persistence

- [x] 2.1 Add EF Core mappings for organizations, contacts, tags, and contact tags.
- [x] 2.2 Add unique constraints for normalized contact email and tag name/category.
- [x] 2.3 Add repositories for contact, organization, and tag aggregate roots.
- [x] 2.4 Create and verify the EF Core migration.

## 3. Application

- [x] 3.1 Add DTOs and use cases for organization CRUD.
- [x] 3.2 Add DTOs and use cases for contact CRUD and filtering.
- [x] 3.3 Add DTOs and use cases for tag CRUD.
- [x] 3.4 Add use cases for assigning and removing contact tags.
- [x] 3.5 Add application tests for create, update, filtering, duplicate prevention, and tagging.

## 4. API

- [x] 4.1 Add REST endpoints for organizations.
- [x] 4.2 Add REST endpoints for contacts.
- [x] 4.3 Add REST endpoints for tags.
- [x] 4.4 Add REST endpoints for assigning and removing contact tags.
- [x] 4.5 Update OpenAPI documentation for all new endpoints and schemas.

## 5. Web

- [x] 5.1 Add Blazor contact list page with search and filters.
- [x] 5.2 Add Blazor contact creation form.
- [x] 5.3 Add basic navigation entries for contacts, organizations, and tags.

## 6. Verification

- [x] 6.1 Add integration tests for EF persistence and constraints.
- [x] 6.2 Add integration tests for main contact, organization, and tag endpoints.
- [x] 6.3 Run `dotnet test`.
- [x] 6.4 Update README or architecture documentation if setup or behavior changes.
