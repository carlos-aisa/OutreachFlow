using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.ContactGroups;

public sealed class ContactGroup
{
    private ContactGroup() { Name = string.Empty; }
    public ContactGroup(string name, DateTimeOffset? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Contact group name is required.");
        Id = Guid.NewGuid(); Name = name.Trim(); CreatedAt = createdAt ?? DateTimeOffset.UtcNow; UpdatedAt = CreatedAt;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void Rename(string name, DateTimeOffset? updatedAt = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Contact group name is required.");
        Name = name.Trim();
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
}
