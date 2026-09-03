using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Application.ContactGroups;

public sealed class ContactGroupService(IContactGroupRepository repository, IUnitOfWork unitOfWork) : IContactGroupService
{
    public async Task<ContactGroupDto> CreateAsync(CreateContactGroupRequest request, CancellationToken cancellationToken = default)
    {
        ContactGroup group;
        try { group = new ContactGroup(request.Name); }
        catch (DomainException exception) { throw new ApplicationValidationException(exception.Message); }
        await repository.AddAsync(group, cancellationToken);
        foreach (var criterion in request.Criteria)
        {
            await repository.AddCriterionAsync(new ContactGroupCriterion(group.Id, criterion.Type, criterion.Value), cancellationToken);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(group, request.Criteria);
    }

    public async Task<ContactGroupDto> GetByIdAsync(Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        var group = await FindGroupAsync(contactGroupId, cancellationToken);
        var criteria = await repository.ListCriteriaAsync(contactGroupId, cancellationToken);
        return ToDto(group, criteria.Select(ToRequest).ToArray());
    }

    public async Task<ContactGroupDto> UpdateAsync(Guid contactGroupId, UpdateContactGroupRequest request, CancellationToken cancellationToken = default)
    {
        var group = await FindGroupAsync(contactGroupId, cancellationToken);
        try { group.Rename(request.Name); }
        catch (DomainException exception) { throw new ApplicationValidationException(exception.Message); }
        var criteria = request.Criteria.Select(item => new ContactGroupCriterion(group.Id, item.Type, item.Value)).ToArray();
        await repository.ReplaceCriteriaAsync(group.Id, criteria, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(group, request.Criteria);
    }

    public async Task DeleteAsync(Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        repository.Remove(await FindGroupAsync(contactGroupId, cancellationToken));
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContactGroupDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var groups = await repository.ListAsync(cancellationToken);
        var result = new List<ContactGroupDto>();
        foreach (var group in groups)
        {
            var criteria = await repository.ListCriteriaAsync(group.Id, cancellationToken);
            result.Add(ToDto(group, criteria.Select(ToRequest).ToArray()));
        }
        return result;
    }

    public async Task<IReadOnlyList<ContactGroupMemberDto>> ListMembersAsync(Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        _ = await FindGroupAsync(contactGroupId, cancellationToken);
        var criteria = await repository.ListCriteriaAsync(contactGroupId, cancellationToken);
        var overrides = await repository.ListOverridesAsync(contactGroupId, cancellationToken);
        var overrideByContact = overrides.ToDictionary(item => item.ContactId);
        var members = new List<ContactGroupMemberDto>();
        foreach (var contact in await repository.ListEvaluationContactsAsync(cancellationToken))
        {
            overrideByContact.TryGetValue(contact.ContactId, out var overrideItem);
            var matches = Matches(contact, criteria);
            if (overrideItem?.Type == ContactGroupOverrideType.Exclude || (overrideItem is null && !matches)) continue;
            members.Add(new ContactGroupMemberDto(contact.ContactId, overrideItem?.Type == ContactGroupOverrideType.Include, overrideItem?.Type == ContactGroupOverrideType.Exclude));
        }
        return members;
    }

    public async Task<IReadOnlyList<ContactGroupMembershipDto>> ListMembershipStatusAsync(Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        _ = await FindGroupAsync(contactGroupId, cancellationToken);
        var criteria = await repository.ListCriteriaAsync(contactGroupId, cancellationToken);
        var overrides = await repository.ListOverridesAsync(contactGroupId, cancellationToken);
        var overrideByContact = overrides.ToDictionary(item => item.ContactId);
        var statuses = new List<ContactGroupMembershipDto>();

        foreach (var contact in await repository.ListEvaluationContactsAsync(cancellationToken))
        {
            overrideByContact.TryGetValue(contact.ContactId, out var overrideItem);
            var status = ClassifyStatus(overrideItem, Matches(contact, criteria));
            statuses.Add(new ContactGroupMembershipDto(contact.ContactId, status));
        }

        return statuses;
    }

    public async Task SetOverrideAsync(Guid contactGroupId, Guid contactId, ContactGroupOverrideType type, CancellationToken cancellationToken = default)
    {
        _ = await FindGroupAsync(contactGroupId, cancellationToken);
        await repository.UpsertOverrideAsync(new ContactGroupMembershipOverride(contactGroupId, contactId, type), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearOverrideAsync(Guid contactGroupId, Guid contactId, CancellationToken cancellationToken = default)
    {
        _ = await FindGroupAsync(contactGroupId, cancellationToken);
        await repository.RemoveOverrideAsync(contactGroupId, contactId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContactGroupDto>> ListForContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        var groups = await ListAsync(cancellationToken);
        var result = new List<ContactGroupDto>();
        foreach (var group in groups)
        {
            if ((await ListMembersAsync(group.Id, cancellationToken)).Any(member => member.ContactId == contactId)) result.Add(group);
        }
        return result;
    }

    private async Task<ContactGroup> FindGroupAsync(Guid contactGroupId, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(contactGroupId, cancellationToken) ?? throw new ApplicationNotFoundException("Contact group was not found.");

    private static ContactGroupDto ToDto(ContactGroup group, IReadOnlyList<ContactGroupCriterionRequest> criteria) =>
        new(group.Id, group.Name, group.CreatedAt, group.UpdatedAt, criteria);

    private static ContactGroupCriterionRequest ToRequest(ContactGroupCriterion item) => new(item.Type, item.Value);

    private static ContactGroupMembershipStatus ClassifyStatus(ContactGroupMembershipOverride? overrideItem, bool matches)
    {
        if (overrideItem?.Type == ContactGroupOverrideType.Include) return ContactGroupMembershipStatus.MemberByManualInclusion;
        if (overrideItem?.Type == ContactGroupOverrideType.Exclude) return ContactGroupMembershipStatus.ExcludedManually;
        return matches ? ContactGroupMembershipStatus.MemberByCriteria : ContactGroupMembershipStatus.NotAMember;
    }

    private static bool Matches(ContactGroupEvaluationContact contact, IReadOnlyList<ContactGroupCriterion> criteria) =>
        MatchesText(contact.Province, criteria, ContactGroupCriterionType.Province) && MatchesText(contact.City, criteria, ContactGroupCriterionType.City) && MatchesText(contact.OrganizationType, criteria, ContactGroupCriterionType.OrganizationType) && MatchesTags(contact.TagIds, criteria);

    private static bool MatchesText(string? value, IReadOnlyList<ContactGroupCriterion> criteria, ContactGroupCriterionType type)
    {
        var values = criteria.Where(item => item.Type == type).Select(item => item.NormalizedValue).ToArray();
        return values.Length == 0 || (value is not null && values.Contains(value.Trim().ToUpperInvariant()));
    }

    private static bool MatchesTags(IReadOnlySet<Guid> tagIds, IReadOnlyList<ContactGroupCriterion> criteria)
    {
        var ids = criteria.Where(item => item.Type == ContactGroupCriterionType.Tag).Select(item => Guid.TryParse(item.Value, out var id) ? id : Guid.Empty).Where(id => id != Guid.Empty).ToHashSet();
        return ids.Count == 0 || tagIds.Overlaps(ids);
    }
}
