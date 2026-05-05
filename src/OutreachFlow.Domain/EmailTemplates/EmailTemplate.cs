using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.EmailTemplates;

public sealed class EmailTemplate
{
    private EmailTemplate()
    {
        Name = string.Empty;
        SubjectTemplate = string.Empty;
        BodyTemplate = string.Empty;
    }

    public EmailTemplate(
        string name,
        string? description,
        string subjectTemplate,
        string bodyTemplate,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name, "Email template name is required.");
        Description = NormalizeOptional(description);
        SubjectTemplate = RequireText(subjectTemplate, "Email template subject is required.");
        BodyTemplate = RequireText(bodyTemplate, "Email template body is required.");
        IsActive = true;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public string SubjectTemplate { get; private set; }

    public string BodyTemplate { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Update(
        string name,
        string? description,
        string subjectTemplate,
        string bodyTemplate,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Name = RequireText(name, "Email template name is required.");
        Description = NormalizeOptional(description);
        SubjectTemplate = RequireText(subjectTemplate, "Email template subject is required.");
        BodyTemplate = RequireText(bodyTemplate, "Email template body is required.");
        IsActive = isActive;
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
