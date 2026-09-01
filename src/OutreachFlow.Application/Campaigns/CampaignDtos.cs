using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Application.Campaigns;

public sealed record CampaignDto(
    Guid Id,
    string Name,
    string? Description,
    Guid EmailTemplateId,
    CampaignStatus Status,
    IReadOnlyList<Guid> AudienceGroupIds,
    bool FollowUpEnabled,
    int FollowUpDueDays,
    FollowUpTaskType FollowUpType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateCampaignRequest(
    string Name,
    string? Description,
    Guid EmailTemplateId,
    IReadOnlyList<Guid> AudienceGroupIds,
    bool FollowUpEnabled,
    int FollowUpDueDays,
    FollowUpTaskType FollowUpType);

public sealed record UpdateCampaignRequest(
    string Name,
    string? Description,
    Guid EmailTemplateId,
    bool FollowUpEnabled,
    int FollowUpDueDays,
    FollowUpTaskType FollowUpType);
