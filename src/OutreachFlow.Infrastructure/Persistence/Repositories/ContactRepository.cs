using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class ContactRepository(OutreachFlowDbContext dbContext) : IContactRepository
{
    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        await dbContext.Contacts.AddAsync(contact, cancellationToken);
    }

    public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Contacts
            .Include(contact => contact.Tags)
            .FirstOrDefaultAsync(contact => contact.Id == id, cancellationToken);
    }

    public async Task<Contact?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Contacts
            .FirstOrDefaultAsync(contact => contact.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async Task<IReadOnlyList<Contact>> ListAsync(
        ContactFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Contacts
            .Include(contact => contact.Tags)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(contact =>
                contact.DisplayName.Contains(search) ||
                contact.Email.Contains(search) ||
                (contact.Role != null && contact.Role.Contains(search)));
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

        return await query
            .OrderBy(contact => contact.DisplayName)
            .ToArrayAsync(cancellationToken);
    }

    public void Remove(Contact contact)
    {
        dbContext.Contacts.Remove(contact);
    }
}
