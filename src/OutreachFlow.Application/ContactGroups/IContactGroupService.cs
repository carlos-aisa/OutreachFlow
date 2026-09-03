using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Application.ContactGroups;

public interface IContactGroupService
{
    Task<ContactGroupDto> CreateAsync(CreateContactGroupRequest request, CancellationToken cancellationToken = default);
    Task<ContactGroupDto> GetByIdAsync(Guid contactGroupId, CancellationToken cancellationToken = default);
    Task<ContactGroupDto> UpdateAsync(Guid contactGroupId, UpdateContactGroupRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid contactGroupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupMemberDto>> ListMembersAsync(Guid contactGroupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupMembershipDto>> ListMembershipStatusAsync(Guid contactGroupId, CancellationToken cancellationToken = default);
    Task SetOverrideAsync(Guid contactGroupId, Guid contactId, ContactGroupOverrideType type, CancellationToken cancellationToken = default);
    Task ClearOverrideAsync(Guid contactGroupId, Guid contactId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactGroupDto>> ListForContactAsync(Guid contactId, CancellationToken cancellationToken = default);
}
