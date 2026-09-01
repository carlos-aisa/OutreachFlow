namespace OutreachFlow.Application.Campaigns;

public interface ICampaignService
{
    Task<CampaignDto> CreateAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default);

    Task<CampaignDto> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignDto>> ListAsync(CancellationToken cancellationToken = default);

    Task<CampaignDto> UpdateAsync(Guid campaignId, UpdateCampaignRequest request, CancellationToken cancellationToken = default);

    Task<CampaignDto> AddAudienceGroupAsync(Guid campaignId, Guid contactGroupId, CancellationToken cancellationToken = default);

    Task<CampaignDto> RemoveAudienceGroupAsync(Guid campaignId, Guid contactGroupId, CancellationToken cancellationToken = default);

    Task<CampaignDto> CloseAsync(Guid campaignId, CancellationToken cancellationToken = default);

    Task<CampaignDto> ReopenAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
