using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Application.Campaigns;

public sealed record CampaignRecipientDto(
    Guid Id,
    Guid CampaignId,
    Guid ContactId,
    string ContactDisplayName,
    string ContactEmail,
    Guid MessageTemplateId,
    CampaignRecipientStatus Status,
    Guid? EmailDraftId,
    string? ExclusionReason,
    DateTimeOffset IncorporatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CampaignRecipientCandidateDto(
    Guid ContactId,
    string DisplayName,
    string Email);

public sealed record GenerateCampaignDraftsRequest(
    Guid SenderProfileId,
    IReadOnlyList<Guid>? AttachmentAssetIds);

public sealed record GenerateCampaignDraftsResult(
    int RequestedRecipients,
    int GeneratedDrafts,
    int ExcludedRecipients,
    IReadOnlyList<CampaignRecipientDto> Recipients);
