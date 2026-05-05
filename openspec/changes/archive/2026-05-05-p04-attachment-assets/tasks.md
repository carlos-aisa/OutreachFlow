## 1. Domain Model

- [x] 1.1 Add `AttachmentAsset` domain model.
- [x] 1.2 Add `EmailTemplateAttachment` relationship model.
- [x] 1.3 Add domain tests for active state and template attachment association rules.

## 2. Persistence and Storage

- [x] 2.1 Add EF Core mappings for attachments and template attachments.
- [x] 2.2 Add file storage application port.
- [x] 2.3 Add local file storage implementation in Infrastructure.
- [x] 2.4 Add safe storage root configuration.
- [x] 2.5 Create and verify the EF Core migration.

## 3. Application

- [x] 3.1 Add DTOs and use cases for attachment asset CRUD/upload.
- [x] 3.2 Add use cases for template default attachment assignment and removal.
- [x] 3.3 Add application tests for upload metadata and inactive attachment rejection.

## 4. API

- [x] 4.1 Add REST endpoints for attachment assets.
- [x] 4.2 Add REST endpoints for template default attachment assignment.
- [x] 4.3 Update OpenAPI documentation.

## 5. Web

- [x] 5.1 Add Blazor attachments list and upload form.
- [x] 5.2 Update template editor to manage default attachments.

## 6. Verification

- [x] 6.1 Add EF integration tests for attachment persistence.
- [x] 6.2 Add API integration tests for upload and template attachment assignment.
- [x] 6.3 Run `dotnet test`.
- [x] 6.4 Update README/configuration documentation for local attachment storage.
