using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.ContactActivities;

public sealed class ContactActivity
{
    private ContactActivity()
    {
        Subject = string.Empty;
    }

    private ContactActivity(
        Guid contactId,
        Guid? organizationId,
        ContactActivityType type,
        string? subject,
        string? bodyPreview,
        string? metadataJson,
        DateTimeOffset occurredAt)
    {
        if (contactId == Guid.Empty)
        {
            throw new DomainException("Contact id is required.");
        }

        Id = Guid.NewGuid();
        ContactId = contactId;
        OrganizationId = organizationId;
        Type = type;
        Subject = NormalizeOptional(subject);
        BodyPreview = NormalizeOptional(bodyPreview);
        MetadataJson = NormalizeOptional(metadataJson);
        OccurredAt = occurredAt;
    }

    public Guid Id { get; private set; }

    public Guid ContactId { get; private set; }

    public Guid? OrganizationId { get; private set; }

    public ContactActivityType Type { get; private set; }

    public string? Subject { get; private set; }

    public string? BodyPreview { get; private set; }

    public string? MetadataJson { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    public static ContactActivity Create(
        Guid contactId,
        Guid? organizationId,
        ContactActivityType type,
        string? subject,
        string? bodyPreview,
        string? metadataJson,
        DateTimeOffset occurredAt)
    {
        return new ContactActivity(
            contactId,
            organizationId,
            type,
            subject,
            bodyPreview,
            metadataJson,
            occurredAt);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
