using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.EmailMessages;

public sealed class EmailMessage
{
    private EmailMessage()
    {
        To = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
        Provider = string.Empty;
    }

    private EmailMessage(
        Guid contactId,
        Guid? organizationId,
        Guid? emailDraftId,
        string to,
        string subject,
        string body,
        EmailMessageStatus status,
        string provider,
        string? providerMessageId,
        DateTimeOffset? sentAt,
        string? failureReason,
        DateTimeOffset createdAt)
    {
        if (contactId == Guid.Empty)
        {
            throw new DomainException("Contact id is required.");
        }

        Id = Guid.NewGuid();
        ContactId = contactId;
        OrganizationId = organizationId;
        EmailDraftId = emailDraftId;
        To = RequireText(to, "Message recipient is required.");
        Subject = RequireText(subject, "Message subject is required.");
        Body = RequireText(body, "Message body is required.");
        Status = status;
        Provider = RequireText(provider, "Message provider is required.");
        ProviderMessageId = NormalizeOptional(providerMessageId);
        SentAt = sentAt;
        FailureReason = NormalizeOptional(failureReason);
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ContactId { get; private set; }

    public Guid? OrganizationId { get; private set; }

    public Guid? EmailDraftId { get; private set; }

    public string To { get; private set; }

    public string Subject { get; private set; }

    public string Body { get; private set; }

    public EmailMessageStatus Status { get; private set; }

    public string Provider { get; private set; }

    public string? ProviderMessageId { get; private set; }

    public DateTimeOffset? SentAt { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static EmailMessage CreateSent(
        Guid contactId,
        Guid? organizationId,
        Guid? emailDraftId,
        string to,
        string subject,
        string body,
        string provider,
        string? providerMessageId,
        DateTimeOffset sentAt)
    {
        return new EmailMessage(
            contactId,
            organizationId,
            emailDraftId,
            to,
            subject,
            body,
            EmailMessageStatus.Sent,
            provider,
            providerMessageId,
            sentAt,
            failureReason: null,
            createdAt: sentAt);
    }

    public static EmailMessage CreateFailed(
        Guid contactId,
        Guid? organizationId,
        Guid? emailDraftId,
        string to,
        string subject,
        string body,
        string provider,
        string failureReason,
        DateTimeOffset occurredAt)
    {
        return new EmailMessage(
            contactId,
            organizationId,
            emailDraftId,
            to,
            subject,
            body,
            EmailMessageStatus.Failed,
            provider,
            providerMessageId: null,
            sentAt: null,
            failureReason,
            createdAt: occurredAt);
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
