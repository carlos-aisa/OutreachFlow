using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;
using System.Text.RegularExpressions;

namespace OutreachFlow.Domain.EmailDrafts;

public sealed class EmailDraft
{
    private static readonly Regex UnresolvedTokenRegex = new(
        @"\{\{[^{}]+\}\}",
        RegexOptions.Compiled);

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

    public DateTimeOffset? ApprovedAt { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

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

    public void UpdateContent(string subject, string body, DateTimeOffset updatedAt)
    {
        EnsureCanEdit();

        Subject = RequireText(subject, "Draft subject is required.");
        Body = RequireText(body, "Draft body is required.");

        if (ContainsUnresolvedTokens(Subject) || ContainsUnresolvedTokens(Body))
        {
            HasRenderErrors = true;
            Status = EmailDraftStatus.NeedsReview;
            ApprovedAt = null;
            SentAt = null;
            FailureReason = null;
            UpdatedAt = updatedAt;
            return;
        }

        HasRenderErrors = false;
        MissingVariablesJson = null;
        UnknownVariablesJson = null;
        Status = EmailDraftStatus.Draft;
        ApprovedAt = null;
        SentAt = null;
        FailureReason = null;
        UpdatedAt = updatedAt;
    }

    public void Approve(DateTimeOffset approvedAt)
    {
        if (Status == EmailDraftStatus.Approved)
        {
            throw new DomainException("Draft is already approved.");
        }

        if (Status == EmailDraftStatus.Sent)
        {
            throw new DomainException("Sent drafts cannot be approved.");
        }

        if (Status == EmailDraftStatus.Cancelled)
        {
            throw new DomainException("Cancelled drafts cannot be approved.");
        }

        if (HasRenderErrors)
        {
            throw new DomainException("Draft cannot be approved while render errors remain.");
        }

        if (!string.IsNullOrWhiteSpace(MissingVariablesJson) || !string.IsNullOrWhiteSpace(UnknownVariablesJson))
        {
            throw new DomainException("Draft cannot be approved while render diagnostics remain unresolved.");
        }

        if (ContainsUnresolvedTokens(Subject) || ContainsUnresolvedTokens(Body))
        {
            throw new DomainException("Draft cannot be approved while unresolved template variables remain.");
        }

        Status = EmailDraftStatus.Approved;
        ApprovedAt = approvedAt;
        FailureReason = null;
        UpdatedAt = approvedAt;
    }

    public void MarkSent(DateTimeOffset sentAt)
    {
        if (Status != EmailDraftStatus.Approved)
        {
            throw new DomainException("Only approved drafts can be marked as sent.");
        }

        Status = EmailDraftStatus.Sent;
        SentAt = sentAt;
        FailureReason = null;
        UpdatedAt = sentAt;
    }

    public void MarkFailed(string failureReason, DateTimeOffset failedAt)
    {
        if (Status != EmailDraftStatus.Approved)
        {
            throw new DomainException("Only approved drafts can be marked as failed.");
        }

        Status = EmailDraftStatus.Failed;
        FailureReason = RequireText(failureReason, "Failure reason is required.");
        UpdatedAt = failedAt;
    }

    public void Cancel(DateTimeOffset cancelledAt)
    {
        if (Status == EmailDraftStatus.Cancelled)
        {
            throw new DomainException("Draft is already cancelled.");
        }

        if (Status == EmailDraftStatus.Sent)
        {
            throw new DomainException("Sent drafts cannot be cancelled.");
        }

        Status = EmailDraftStatus.Cancelled;
        CancelledAt = cancelledAt;
        UpdatedAt = cancelledAt;
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

    private static bool ContainsUnresolvedTokens(string value)
    {
        return UnresolvedTokenRegex.IsMatch(value);
    }

    private void EnsureCanEdit()
    {
        if (Status == EmailDraftStatus.Sent || Status == EmailDraftStatus.Cancelled)
        {
            throw new DomainException("Sent or cancelled drafts cannot be edited.");
        }
    }
}
