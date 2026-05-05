using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Contacts;

public sealed class Contact
{
    private readonly List<ContactTag> _tags = [];

    private Contact()
    {
        DisplayName = string.Empty;
        Email = string.Empty;
        NormalizedEmail = string.Empty;
    }

    public Contact(
        string displayName,
        string email,
        Guid? organizationId = null,
        string? phone = null,
        string? role = null,
        string? source = null,
        ContactStatus status = ContactStatus.New,
        bool doNotContact = false,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        DisplayName = RequireDisplayName(displayName);
        Email = EmailAddress.RequireValid(email);
        NormalizedEmail = EmailAddress.Normalize(email);
        Phone = NormalizeOptional(phone);
        Role = NormalizeOptional(role);
        Source = NormalizeOptional(source);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        Status = status;
        DoNotContact = doNotContact || status == ContactStatus.DoNotContact;
    }

    public Guid Id { get; private set; }

    public Guid? OrganizationId { get; private set; }

    public string DisplayName { get; private set; }

    public string Email { get; private set; }

    public string NormalizedEmail { get; private set; }

    public string? Phone { get; private set; }

    public string? Role { get; private set; }

    public string? Source { get; private set; }

    public ContactStatus Status { get; private set; }

    public bool DoNotContact { get; private set; }

    public DateTimeOffset? LastContactedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<ContactTag> Tags => _tags.AsReadOnly();

    public void Update(
        string displayName,
        string email,
        Guid? organizationId,
        string? phone,
        string? role,
        string? source,
        ContactStatus status,
        bool doNotContact,
        DateTimeOffset updatedAt)
    {
        DisplayName = RequireDisplayName(displayName);
        Email = EmailAddress.RequireValid(email);
        NormalizedEmail = EmailAddress.Normalize(email);
        OrganizationId = organizationId;
        Phone = NormalizeOptional(phone);
        Role = NormalizeOptional(role);
        Source = NormalizeOptional(source);
        ChangeStatus(status, updatedAt);

        if (doNotContact)
        {
            MarkDoNotContact(updatedAt);
        }
        else if (status != ContactStatus.DoNotContact)
        {
            DoNotContact = false;
        }

        UpdatedAt = updatedAt;
    }

    public void ChangeStatus(ContactStatus status, DateTimeOffset updatedAt)
    {
        Status = status;

        if (status == ContactStatus.DoNotContact)
        {
            DoNotContact = true;
        }

        UpdatedAt = updatedAt;
    }

    public void MarkDoNotContact(DateTimeOffset updatedAt)
    {
        DoNotContact = true;
        Status = ContactStatus.DoNotContact;
        UpdatedAt = updatedAt;
    }

    public void MarkContacted(DateTimeOffset contactedAt)
    {
        LastContactedAt = contactedAt;

        if (Status == ContactStatus.New)
        {
            Status = ContactStatus.Contacted;
        }

        UpdatedAt = contactedAt;
    }

    public bool AssignTag(Guid tagId)
    {
        if (_tags.Any(tag => tag.TagId == tagId))
        {
            return false;
        }

        _tags.Add(new ContactTag(Id, tagId));
        return true;
    }

    public bool RemoveTag(Guid tagId)
    {
        var existingTag = _tags.FirstOrDefault(tag => tag.TagId == tagId);

        if (existingTag is null)
        {
            return false;
        }

        _tags.Remove(existingTag);
        return true;
    }

    private static string RequireDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Contact display name is required.");
        }

        return displayName.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
