using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Attachments;

namespace OutreachFlow.Domain.EmailTemplates;

public sealed class EmailTemplate
{
    private readonly List<EmailTemplateAttachment> _defaultAttachments = [];

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

    public IReadOnlyCollection<EmailTemplateAttachment> DefaultAttachments => _defaultAttachments.AsReadOnly();

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

    public bool AssignDefaultAttachment(AttachmentAsset attachmentAsset, DateTimeOffset updatedAt)
    {
        ArgumentNullException.ThrowIfNull(attachmentAsset);

        if (!attachmentAsset.IsActive)
        {
            throw new DomainException("Inactive attachments cannot be assigned to templates.");
        }

        if (_defaultAttachments.Any(defaultAttachment =>
                defaultAttachment.AttachmentAssetId == attachmentAsset.Id))
        {
            return false;
        }

        _defaultAttachments.Add(new EmailTemplateAttachment(Id, attachmentAsset.Id));
        UpdatedAt = updatedAt;
        return true;
    }

    public bool RemoveDefaultAttachment(Guid attachmentAssetId, DateTimeOffset updatedAt)
    {
        var existingAttachment = _defaultAttachments.FirstOrDefault(defaultAttachment =>
            defaultAttachment.AttachmentAssetId == attachmentAssetId);

        if (existingAttachment is null)
        {
            return false;
        }

        _defaultAttachments.Remove(existingAttachment);
        UpdatedAt = updatedAt;
        return true;
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
