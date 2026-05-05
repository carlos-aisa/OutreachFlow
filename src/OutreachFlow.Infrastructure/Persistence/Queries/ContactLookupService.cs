using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Infrastructure.Persistence.Queries;

public sealed class ContactLookupService(OutreachFlowDbContext dbContext) : IContactLookupService
{
    public async Task<ContactDto> MapAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        var mappedContacts = await MapAsync([contact], cancellationToken);
        return mappedContacts.Single();
    }

    public async Task<IReadOnlyList<ContactDto>> MapAsync(
        IReadOnlyList<Contact> contacts,
        CancellationToken cancellationToken = default)
    {
        var organizationIds = contacts
            .Select(contact => contact.OrganizationId)
            .OfType<Guid>()
            .Distinct()
            .ToArray();

        var tagIds = contacts
            .SelectMany(contact => contact.Tags.Select(tag => tag.TagId))
            .Distinct()
            .ToArray();

        var organizations = await dbContext.Organizations
            .Where(organization => organizationIds.Contains(organization.Id))
            .ToDictionaryAsync(
                organization => organization.Id,
                organization => organization.Name,
                cancellationToken);

        var tags = await dbContext.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToDictionaryAsync(
                tag => tag.Id,
                tag => new ContactTagDto(tag.Id, tag.Name, tag.Category),
                cancellationToken);

        return contacts.Select(contact =>
        {
            var organizationName = contact.OrganizationId is Guid organizationId &&
                organizations.TryGetValue(organizationId, out var name)
                    ? name
                    : null;

            var contactTags = contact.Tags
                .Select(contactTag => tags.GetValueOrDefault(contactTag.TagId))
                .Where(tag => tag is not null)
                .Select(tag => tag!)
                .OrderBy(tag => tag.Category)
                .ThenBy(tag => tag.Name)
                .ToArray();

            return new ContactDto(
                contact.Id,
                contact.OrganizationId,
                organizationName,
                contact.DisplayName,
                contact.Email,
                contact.Phone,
                contact.Role,
                contact.Source,
                contact.Status,
                contact.DoNotContact,
                contact.LastContactedAt,
                contact.CreatedAt,
                contact.UpdatedAt,
                contactTags);
        }).ToArray();
    }
}
