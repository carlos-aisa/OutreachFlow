using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Application.ContactGroups;

public interface IContactGroupRepository
{
    Task AddAsync(ContactGroup contactGroup, CancellationToken cancellationToken = default);
    Task<ContactGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroup>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupCriterion>> ListCriteriaAsync(Guid contactGroupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupMembershipOverride>> ListOverridesAsync(Guid contactGroupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupEvaluationContact>> ListEvaluationContactsAsync(CancellationToken cancellationToken = default);
    Task AddCriterionAsync(ContactGroupCriterion criterion, CancellationToken cancellationToken = default);
    Task ReplaceCriteriaAsync(Guid contactGroupId, IReadOnlyList<ContactGroupCriterion> criteria, CancellationToken cancellationToken = default);
    Task UpsertOverrideAsync(ContactGroupMembershipOverride membershipOverride, CancellationToken cancellationToken = default);
    Task RemoveOverrideAsync(Guid contactGroupId, Guid contactId, CancellationToken cancellationToken = default);
    void Remove(ContactGroup contactGroup);
}

public sealed record ContactGroupEvaluationContact(
    Guid ContactId,
    string? Province,
    string? City,
    string? OrganizationType,
    IReadOnlySet<Guid> TagIds);
