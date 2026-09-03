using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class ContactGroupRepository(OutreachFlowDbContext dbContext) : IContactGroupRepository
{
    public async Task AddAsync(ContactGroup contactGroup, CancellationToken cancellationToken = default) =>
        await dbContext.ContactGroups.AddAsync(contactGroup, cancellationToken);

    public Task<ContactGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.ContactGroups.FirstOrDefaultAsync(group => group.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ContactGroup>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.ContactGroups.OrderBy(group => group.Name).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<ContactGroupCriterion>> ListCriteriaAsync(Guid contactGroupId, CancellationToken cancellationToken = default) =>
        await dbContext.ContactGroupCriteria.Where(item => item.ContactGroupId == contactGroupId).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<ContactGroupMembershipOverride>> ListOverridesAsync(Guid contactGroupId, CancellationToken cancellationToken = default) =>
        await dbContext.ContactGroupMembershipOverrides.Where(item => item.ContactGroupId == contactGroupId).ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyList<ContactGroupEvaluationContact>> ListEvaluationContactsAsync(CancellationToken cancellationToken = default)
    {
        var contacts = await dbContext.Contacts.Include(contact => contact.Tags).ToArrayAsync(cancellationToken);
        var organizations = await dbContext.Organizations.ToDictionaryAsync(item => item.Id, cancellationToken);
        return contacts.Select(contact =>
        {
            organizations.TryGetValue(contact.OrganizationId ?? Guid.Empty, out var organization);
            return new ContactGroupEvaluationContact(contact.Id, organization?.Province, organization?.City, organization?.Type, contact.Tags.Select(tag => tag.TagId).ToHashSet());
        }).ToArray();
    }

    public async Task AddCriterionAsync(ContactGroupCriterion criterion, CancellationToken cancellationToken = default) =>
        await dbContext.ContactGroupCriteria.AddAsync(criterion, cancellationToken);

    public async Task ReplaceCriteriaAsync(Guid contactGroupId, IReadOnlyList<ContactGroupCriterion> criteria, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.ContactGroupCriteria.Where(item => item.ContactGroupId == contactGroupId).ToArrayAsync(cancellationToken);
        dbContext.ContactGroupCriteria.RemoveRange(existing);
        await dbContext.ContactGroupCriteria.AddRangeAsync(criteria, cancellationToken);
    }

    public async Task UpsertOverrideAsync(ContactGroupMembershipOverride membershipOverride, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.ContactGroupMembershipOverrides.FindAsync(
            [membershipOverride.ContactGroupId, membershipOverride.ContactId],
            cancellationToken);
        if (existing is not null) dbContext.ContactGroupMembershipOverrides.Remove(existing);
        await dbContext.ContactGroupMembershipOverrides.AddAsync(membershipOverride, cancellationToken);
    }

    public async Task RemoveOverrideAsync(Guid contactGroupId, Guid contactId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.ContactGroupMembershipOverrides.FindAsync(
            [contactGroupId, contactId],
            cancellationToken);
        if (existing is not null) dbContext.ContactGroupMembershipOverrides.Remove(existing);
    }

    public void Remove(ContactGroup contactGroup) => dbContext.ContactGroups.Remove(contactGroup);
}
