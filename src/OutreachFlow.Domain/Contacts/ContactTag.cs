namespace OutreachFlow.Domain.Contacts;

public sealed class ContactTag
{
    private ContactTag()
    {
    }

    public ContactTag(Guid contactId, Guid tagId)
    {
        ContactId = contactId;
        TagId = tagId;
    }

    public Guid ContactId { get; private set; }

    public Guid TagId { get; private set; }
}
