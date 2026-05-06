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
        SenderSignatureFormat? signatureFormat = null,
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
        SignatureFormat = signatureFormat;
        ValidateSignature();
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

    public SenderSignatureFormat? SignatureFormat { get; private set; }

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
        SenderSignatureFormat? signatureFormat,
        DateTimeOffset updatedAt)
    {
        Name = RequireText(name, "Sender profile name is required.");
        Email = EmailAddress.RequireValid(email);
        NormalizedEmail = EmailAddress.Normalize(email);
        Phone = NormalizeOptional(phone);
        OrganizationName = NormalizeOptional(organizationName);
        Website = NormalizeOptional(website);
        Signature = NormalizeOptional(signature);
        SignatureFormat = signatureFormat;
        ValidateSignature();
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

    private void ValidateSignature()
    {
        if (Signature is null && SignatureFormat is null)
        {
            return;
        }

        if (Signature is null && SignatureFormat is not null)
        {
            throw new DomainException("Signature content is required when signature format is provided.");
        }

        if (Signature is not null && SignatureFormat is null)
        {
            throw new DomainException("Signature format is required when signature content is provided.");
        }

        if (Signature!.Length > 12000)
        {
            throw new DomainException("Signature content cannot exceed 12000 characters.");
        }

        if (!Enum.IsDefined(SignatureFormat!.Value))
        {
            throw new DomainException("Signature format is not supported.");
        }

        if (SignatureFormat == SenderSignatureFormat.Html)
        {
            var trimmed = Signature.Trim();
            if (!trimmed.StartsWith('<') || !trimmed.EndsWith('>'))
            {
                throw new DomainException("HTML signature content must start and end with an HTML tag.");
            }

            return;
        }

        if (SignatureFormat == SenderSignatureFormat.Rtf)
        {
            var trimmed = Signature.TrimStart();
            if (!trimmed.StartsWith(@"{\rtf", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException("RTF signature content must start with '{\\rtf'.");
            }
        }
    }
}
