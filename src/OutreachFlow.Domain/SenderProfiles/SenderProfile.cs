using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Domain.SenderProfiles;

public sealed class SenderProfile
{
    private SenderProfile()
    {
        Name = string.Empty;
        Email = string.Empty;
        NormalizedEmail = string.Empty;
    }

    public SenderProfile(
        string name,
        string email,
        string? phone = null,
        string? organizationName = null,
        string? website = null,
        string? signature = null,
        bool isDefault = false,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name, "Sender profile name is required.");
        Email = EmailAddress.RequireValid(email);
        NormalizedEmail = EmailAddress.Normalize(email);
        Phone = NormalizeOptional(phone);
        OrganizationName = NormalizeOptional(organizationName);
        Website = NormalizeOptional(website);
        Signature = NormalizeOptional(signature);
        IsDefault = isDefault;
        IsActive = true;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Email { get; private set; }

    public string NormalizedEmail { get; private set; }

    public string? Phone { get; private set; }

    public string? OrganizationName { get; private set; }

    public string? Website { get; private set; }

    public string? Signature { get; private set; }

    public bool IsDefault { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Update(
        string name,
        string email,
        string? phone,
        string? organizationName,
        string? website,
        string? signature,
        DateTimeOffset updatedAt)
    {
        Name = RequireText(name, "Sender profile name is required.");
        Email = EmailAddress.RequireValid(email);
        NormalizedEmail = EmailAddress.Normalize(email);
        Phone = NormalizeOptional(phone);
        OrganizationName = NormalizeOptional(organizationName);
        Website = NormalizeOptional(website);
        Signature = NormalizeOptional(signature);
        UpdatedAt = updatedAt;
    }

    public void MarkDefault(DateTimeOffset updatedAt)
    {
        if (!IsActive)
        {
            throw new DomainException("Inactive sender profiles cannot be default.");
        }

        IsDefault = true;
        UpdatedAt = updatedAt;
    }

    public void ClearDefault(DateTimeOffset updatedAt)
    {
        IsDefault = false;
        UpdatedAt = updatedAt;
    }

    public void Activate(DateTimeOffset updatedAt)
    {
        IsActive = true;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        IsDefault = false;
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
