using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Tags;

public sealed class Tag
{
    private Tag()
    {
        Name = string.Empty;
        NormalizedName = string.Empty;
    }

    public Tag(string name, string? category = null, DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireName(name);
        NormalizedName = NormalizeKey(Name);
        Category = NormalizeOptional(category);
        NormalizedCategory = NormalizeKey(Category);
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string NormalizedName { get; private set; }

    public string? Category { get; private set; }

    public string NormalizedCategory { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public void Update(string name, string? category)
    {
        Name = RequireName(name);
        NormalizedName = NormalizeKey(Name);
        Category = NormalizeOptional(category);
        NormalizedCategory = NormalizeKey(Category);
    }

    private static string RequireName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Tag name is required.");
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
    }
}
