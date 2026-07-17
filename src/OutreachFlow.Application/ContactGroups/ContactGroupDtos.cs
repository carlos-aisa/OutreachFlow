using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Application.ContactGroups;

public sealed record ContactGroupCriterionRequest(ContactGroupCriterionType Type, string Value);

public sealed record CreateContactGroupRequest(string Name, IReadOnlyList<ContactGroupCriterionRequest> Criteria);
public sealed record UpdateContactGroupRequest(string Name, IReadOnlyList<ContactGroupCriterionRequest> Criteria);

public sealed record ContactGroupDto(Guid Id, string Name, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, IReadOnlyList<ContactGroupCriterionRequest> Criteria);

public sealed record ContactGroupMemberDto(Guid ContactId, bool IsManualInclusion, bool IsManualExclusion);
