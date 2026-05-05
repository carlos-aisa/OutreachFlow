using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;

namespace OutreachFlow.Application.EmailDrafts;

public sealed record EmailDraftDto(
    Guid Id,
    Guid ContactId,
    string ContactDisplayName,
    string ContactEmail,
    Guid? OrganizationId,
    Guid? TemplateId,
    Guid SenderProfileId,
    string Subject,
    string Body,
    EmailDraftStatus Status,
    bool HasRenderErrors,
    IReadOnlyList<string> MissingVariables,
    IReadOnlyList<string> UnknownVariables,
    IReadOnlyList<Guid> AttachmentAssetIds,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset? CancelledAt);

public sealed record EmailDraftFilterRequest(
    EmailDraftStatus? Status,
    Guid? ContactId);

public sealed record GenerateEmailDraftsRequest(
    string? Search,
    Guid? TagId,
    ContactStatus? Status,
    bool? DoNotContact,
    Guid? OrganizationId,
    DateTimeOffset? LastContactedFrom,
    DateTimeOffset? LastContactedTo,
    Guid TemplateId,
    Guid SenderProfileId,
    IReadOnlyList<Guid>? AttachmentAssetIds);

public sealed record UpdateEmailDraftRequest(
    string Subject,
    string Body);

public sealed record SkippedDraftContactDto(
    Guid ContactId,
    string DisplayName,
    string Email,
    string Reason);

public sealed record GenerateEmailDraftsResult(
    int RequestedContacts,
    int GeneratedDrafts,
    int SkippedContacts,
    IReadOnlyList<EmailDraftDto> Drafts,
    IReadOnlyList<SkippedDraftContactDto> Skipped);
