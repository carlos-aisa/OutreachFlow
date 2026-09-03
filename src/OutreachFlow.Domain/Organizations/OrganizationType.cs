using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Organizations;

public sealed class OrganizationType
{
    private OrganizationType()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    public OrganizationType(string name, DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireName(name);
        NormalizedName = NormalizeKey(Name);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string NormalizedName { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public void Update(string name)
    {
        Name = RequireName(name);
        NormalizedName = NormalizeKey(Name);
    }

    private static string RequireName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Organization type name is required.");
        }

        return value.Trim();
    }

    private static string NormalizeKey(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
}
