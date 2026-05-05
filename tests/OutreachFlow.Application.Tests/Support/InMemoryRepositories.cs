using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Application.Tests.Support;

internal sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public int SaveChangesCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCount++;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryOrganizationRepository : IOrganizationRepository
{
    private readonly List<Organization> _organizations = [];

    public IReadOnlyList<Organization> Organizations => _organizations;

    public Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _organizations.Add(organization);
        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_organizations.FirstOrDefault(organization => organization.Id == id));
    }

    public Task<IReadOnlyList<Organization>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Organization>>(
            _organizations.OrderBy(organization => organization.Name).ToArray());
    }

    public void Remove(Organization organization)
    {
        _organizations.Remove(organization);
    }
}

internal sealed class InMemoryTagRepository : ITagRepository
{
    private readonly List<Tag> _tags = [];

    public IReadOnlyList<Tag> Tags => _tags;

    public Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _tags.Add(tag);
        return Task.CompletedTask;
    }

    public Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tags.FirstOrDefault(tag => tag.Id == id));
    }

    public Task<Tag?> GetByNameAsync(
        string name,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeKey(name);
        var normalizedCategory = NormalizeKey(category);

        return Task.FromResult(_tags.FirstOrDefault(tag =>
            tag.NormalizedName == normalizedName &&
            tag.NormalizedCategory == normalizedCategory));
    }

    public Task<IReadOnlyList<Tag>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Tag>>(
            _tags
                .OrderBy(tag => tag.Category)
                .ThenBy(tag => tag.Name)
                .ToArray());
    }

    public void Remove(Tag tag)
    {
        _tags.Remove(tag);
    }

    private static string NormalizeKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
    }
}

internal sealed class InMemoryContactRepository : IContactRepository
{
    private readonly List<Contact> _contacts = [];

    public IReadOnlyList<Contact> Contacts => _contacts;

    public Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        _contacts.Add(contact);
        return Task.CompletedTask;
    }

    public Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_contacts.FirstOrDefault(contact => contact.Id == id));
    }

    public Task<Contact?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_contacts.FirstOrDefault(contact =>
            contact.NormalizedEmail == normalizedEmail));
    }

    public Task<IReadOnlyList<Contact>> ListAsync(
        ContactFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Contact> query = _contacts;

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(contact =>
                contact.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                contact.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (contact.Role is not null &&
                    contact.Role.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (filter.TagId is not null)
        {
            query = query.Where(contact => contact.Tags.Any(tag => tag.TagId == filter.TagId));
        }

        if (filter.Status is not null)
        {
            query = query.Where(contact => contact.Status == filter.Status);
        }

        if (filter.DoNotContact is not null)
        {
            query = query.Where(contact => contact.DoNotContact == filter.DoNotContact);
        }

        if (filter.OrganizationId is not null)
        {
            query = query.Where(contact => contact.OrganizationId == filter.OrganizationId);
        }

        if (filter.LastContactedFrom is not null)
        {
            query = query.Where(contact => contact.LastContactedAt >= filter.LastContactedFrom);
        }

        if (filter.LastContactedTo is not null)
        {
            query = query.Where(contact => contact.LastContactedAt <= filter.LastContactedTo);
        }

        return Task.FromResult<IReadOnlyList<Contact>>(
            query.OrderBy(contact => contact.DisplayName).ToArray());
    }

    public void Remove(Contact contact)
    {
        _contacts.Remove(contact);
    }
}

internal sealed class InMemoryContactLookupService(
    InMemoryOrganizationRepository organizationRepository,
    InMemoryTagRepository tagRepository)
    : IContactLookupService
{
    public Task<ContactDto> MapAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Map(contact));
    }

    public Task<IReadOnlyList<ContactDto>> MapAsync(
        IReadOnlyList<Contact> contacts,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ContactDto>>(contacts.Select(Map).ToArray());
    }

    private ContactDto Map(Contact contact)
    {
        var organizationName = contact.OrganizationId is Guid organizationId
            ? organizationRepository.Organizations.FirstOrDefault(organization => organization.Id == organizationId)?.Name
            : null;

        var tags = contact.Tags
            .Select(contactTag => tagRepository.Tags.FirstOrDefault(tag => tag.Id == contactTag.TagId))
            .Where(tag => tag is not null)
            .Select(tag => new ContactTagDto(tag!.Id, tag.Name, tag.Category))
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
            tags);
    }
}
