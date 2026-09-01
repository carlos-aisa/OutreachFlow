using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Application.Campaigns;

public sealed record CampaignDto(
    Guid Id,
    string Name,
    string? Description,
    Guid EmailTemplateId,
    CampaignStatus Status,
    IReadOnlyList<Guid> AudienceGroupIds,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateCampaignRequest(
    string Name,
    string? Description,
    Guid EmailTemplateId,
    IReadOnlyList<Guid> AudienceGroupIds);

public sealed record UpdateCampaignRequest(
    string Name,
    string? Description,
    Guid EmailTemplateId);
