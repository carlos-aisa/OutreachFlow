using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Organizations;

public sealed class Organization
{
    private Organization()
    {
        Name = string.Empty;
    }

    public Organization(
        string name,
        string? type = null,
        string? website = null,
        string? city = null,
        string? province = null,
        string? country = null,
        string? notes = null,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name, "Organization name is required.");
        Type = NormalizeOptional(type);
        Website = NormalizeOptional(website);
        City = NormalizeOptional(city);
        Province = NormalizeOptional(province);
        Country = NormalizeOptional(country);
        Notes = NormalizeOptional(notes);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string? Type { get; private set; }

    public string? Website { get; private set; }

    public string? City { get; private set; }

    public string? Province { get; private set; }

    public string? Country { get; private set; }

    public string? Notes { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Update(
        string name,
        string? type,
        string? website,
        string? city,
        string? province,
        string? country,
        string? notes,
        DateTimeOffset updatedAt)
    {
        Name = RequireText(name, "Organization name is required.");
        Type = NormalizeOptional(type);
        Website = NormalizeOptional(website);
        City = NormalizeOptional(city);
        Province = NormalizeOptional(province);
        Country = NormalizeOptional(country);
        Notes = NormalizeOptional(notes);
        UpdatedAt = updatedAt;
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
