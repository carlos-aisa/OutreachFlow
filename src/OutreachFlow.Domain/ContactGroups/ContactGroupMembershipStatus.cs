namespace OutreachFlow.Domain.ContactGroups;

public enum ContactGroupMembershipStatus
{
    NotAMember = 0,
    MemberByCriteria = 1,
    MemberByManualInclusion = 2,
    ExcludedManually = 3
}
