using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.EmailDrafts;

public sealed class EmailDraft
{
    private readonly List<EmailDraftAttachment> _attachments = [];

    private EmailDraft()
    {
        Subject = string.Empty;
        Body = string.Empty;
    }

    private EmailDraft(
        Guid contactId,
        Guid? organizationId,
        Guid? templateId,
        Guid senderProfileId,
        string subject,
        string body,
        bool hasRenderErrors,
        string? missingVariablesJson,
        string? unknownVariablesJson,
        DateTimeOffset? createdAt = null)
    {
        if (contactId == Guid.Empty)
        {
            throw new DomainException("Contact id is required.");
        }

        if (senderProfileId == Guid.Empty)
        {
            throw new DomainException("Sender profile id is required.");
        }

        Id = Guid.NewGuid();
        ContactId = contactId;
        OrganizationId = organizationId;
        TemplateId = templateId;
        SenderProfileId = senderProfileId;
        Subject = RequireText(subject, "Draft subject is required.");
        Body = RequireText(body, "Draft body is required.");
        HasRenderErrors = hasRenderErrors;
        MissingVariablesJson = NormalizeOptional(missingVariablesJson);
        UnknownVariablesJson = NormalizeOptional(unknownVariablesJson);
        Status = hasRenderErrors ? EmailDraftStatus.NeedsReview : EmailDraftStatus.Draft;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public Guid ContactId { get; private set; }

    public Guid? OrganizationId { get; private set; }

    public Guid? TemplateId { get; private set; }

    public Guid SenderProfileId { get; private set; }

    public string Subject { get; private set; }

    public string Body { get; private set; }

    public EmailDraftStatus Status { get; private set; }

    public bool HasRenderErrors { get; private set; }

    public string? MissingVariablesJson { get; private set; }

    public string? UnknownVariablesJson { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<EmailDraftAttachment> Attachments => _attachments.AsReadOnly();

    public static EmailDraft CreateGenerated(
        Guid contactId,
        Guid? organizationId,
        Guid templateId,
        Guid senderProfileId,
        string subject,
        string body,
        bool hasRenderErrors,
        string? missingVariablesJson,
        string? unknownVariablesJson,
        DateTimeOffset? createdAt = null)
    {
        return new EmailDraft(
            contactId,
            organizationId,
            templateId,
            senderProfileId,
            subject,
            body,
            hasRenderErrors,
            missingVariablesJson,
            unknownVariablesJson,
            createdAt);
    }

    public bool AssignAttachment(AttachmentAsset attachmentAsset, DateTimeOffset updatedAt)
    {
        ArgumentNullException.ThrowIfNull(attachmentAsset);

        if (!attachmentAsset.IsActive)
        {
            throw new DomainException("Inactive attachments cannot be assigned to drafts.");
        }

        if (_attachments.Any(attachment => attachment.AttachmentAssetId == attachmentAsset.Id))
        {
            return false;
        }

        _attachments.Add(new EmailDraftAttachment(Id, attachmentAsset.Id));
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
