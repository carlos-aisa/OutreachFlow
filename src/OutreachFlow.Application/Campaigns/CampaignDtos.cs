namespace OutreachFlow.Application.Campaigns;
public sealed record CampaignDto(Guid Id, string Name, string? Subject, string? Body, Guid? SenderProfileId, DateTimeOffset? FollowUpDueAt, IReadOnlyList<Guid> ContactIds, IReadOnlyList<Guid> ContactGroupIds, IReadOnlyList<Guid> AttachmentAssetIds, IReadOnlyList<string> MissingPrerequisites, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
public sealed record CreateCampaignRequest(string Name);
public sealed record UpdateCampaignRequest(string Name, string? Subject, string? Body, Guid? SenderProfileId, DateTimeOffset? FollowUpDueAt, IReadOnlyList<Guid> ContactIds, IReadOnlyList<Guid> ContactGroupIds, IReadOnlyList<Guid> AttachmentAssetIds);
