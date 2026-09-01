namespace OutreachFlow.Domain.ContactGroups;

public sealed class ContactGroupMembershipOverride
{
    private ContactGroupMembershipOverride() { }
    public ContactGroupMembershipOverride(Guid contactGroupId, Guid contactId, ContactGroupOverrideType type)
    {
        ContactGroupId = contactGroupId;
        ContactId = contactId;
        Type = type;
    }
    public Guid ContactGroupId { get; private set; }
    public Guid ContactId { get; private set; }
    public ContactGroupOverrideType Type { get; private set; }
}
