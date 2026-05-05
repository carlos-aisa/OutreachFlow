namespace OutreachFlow.Application.EmailSending;

public sealed record SendEmailCommand(
    string To,
    string Subject,
    string Body,
    EmailSenderPayload Sender,
    IReadOnlyList<EmailAttachmentPayload> Attachments,
    IReadOnlyDictionary<string, string>? Metadata);

public sealed record EmailAttachmentPayload(
    Guid AttachmentAssetId,
    string Name,
    string FileName,
    string ContentType,
    string StoragePath,
    long SizeBytes);

public sealed record EmailSenderPayload(
    Guid SenderProfileId,
    string Name,
    string Email,
    string? Phone,
    string? OrganizationName,
    string? Website,
    string? Signature);
