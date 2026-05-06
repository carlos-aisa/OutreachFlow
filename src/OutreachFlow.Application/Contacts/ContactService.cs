using System.Text.Json;

using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.Contacts;

public sealed class ContactService(
    IContactRepository contactRepository,
    IOrganizationRepository organizationRepository,
    ITagRepository tagRepository,
    IContactActivityService contactActivityService,
    IContactLookupService contactLookupService,
    IUnitOfWork unitOfWork)
    : IContactService
{
    public async Task<ContactDto> CreateAsync(
        CreateContactRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureOrganizationExistsAsync(request.OrganizationId, cancellationToken);
        await EnsureEmailIsUniqueAsync(request.Email, null, cancellationToken);

        var contact = CreateContact(request);
        await contactRepository.AddAsync(contact, cancellationToken);
        await contactActivityService.RecordAsync(new CreateContactActivityRequest(
            contact.Id,
            contact.OrganizationId,
            ContactActivityType.ContactCreated,
            Subject: "Contact created",
            BodyPreview: null,
            MetadataJson: null),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await contactLookupService.MapAsync(contact, cancellationToken);
    }

    public async Task<ContactDto> UpdateAsync(
        Guid id,
        UpdateContactRequest request,
        CancellationToken cancellationToken = default)
    {
        var contact = await contactRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");
        var previousStatus = contact.Status;
        var previousDoNotContact = contact.DoNotContact;

        await EnsureOrganizationExistsAsync(request.OrganizationId, cancellationToken);
        await EnsureEmailIsUniqueAsync(request.Email, id, cancellationToken);

        try
        {
            contact.Update(
                request.DisplayName,
                request.Email,
                request.OrganizationId,
                request.Phone,
                request.Role,
                request.Source,
                request.Status,
                request.DoNotContact,
                DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await contactActivityService.RecordAsync(new CreateContactActivityRequest(
            contact.Id,
            contact.OrganizationId,
            ContactActivityType.ContactUpdated,
            Subject: "Contact updated",
            BodyPreview: null,
            MetadataJson: null),
            cancellationToken);

        if (previousStatus != contact.Status || previousDoNotContact != contact.DoNotContact)
        {
            var metadataJson = JsonSerializer.Serialize(new
            {
                previousStatus,
                newStatus = contact.Status,
                previousDoNotContact,
                newDoNotContact = contact.DoNotContact
            });

            await contactActivityService.RecordAsync(new CreateContactActivityRequest(
                contact.Id,
                contact.OrganizationId,
                ContactActivityType.StatusChanged,
                Subject: "Contact status changed",
                BodyPreview: null,
                MetadataJson: metadataJson),
                cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await contactLookupService.MapAsync(contact, cancellationToken);
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await contactRepository.GetByIdAsync(id, cancellationToken);
        return contact is null ? null : await contactLookupService.MapAsync(contact, cancellationToken);
    }

    public async Task<IReadOnlyList<ContactDto>> ListAsync(
        ContactFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var contacts = await contactRepository.ListAsync(filter, cancellationToken);
        return await contactLookupService.MapAsync(contacts, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await contactRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        contactRepository.Remove(contact);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignTagAsync(Guid contactId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var contact = await contactRepository.GetByIdAsync(contactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        _ = await tagRepository.GetByIdAsync(tagId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Tag was not found.");

        contact.AssignTag(tagId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveTagAsync(Guid contactId, Guid tagId, CancellationToken cancellationToken = default)
    {
        var contact = await contactRepository.GetByIdAsync(contactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        contact.RemoveTag(tagId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureOrganizationExistsAsync(
        Guid? organizationId,
        CancellationToken cancellationToken)
    {
        if (organizationId is null)
        {
            return;
        }

        _ = await organizationRepository.GetByIdAsync(organizationId.Value, cancellationToken)
            ?? throw new ApplicationNotFoundException("Organization was not found.");
    }

    private async Task EnsureEmailIsUniqueAsync(
        string email,
        Guid? currentContactId,
        CancellationToken cancellationToken)
    {
        string normalizedEmail;

        try
        {
            normalizedEmail = EmailAddress.Normalize(email);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        var existingContact = await contactRepository.GetByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (existingContact is not null && existingContact.Id != currentContactId)
        {
            throw new ApplicationConflictException("A contact with this email already exists.");
        }
    }

    private static Contact CreateContact(CreateContactRequest request)
    {
        try
        {
            return new Contact(
                request.DisplayName,
                request.Email,
                request.OrganizationId,
                request.Phone,
                request.Role,
                request.Source,
                request.Status,
                request.DoNotContact);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }
}
