namespace OutreachFlow.Application.Campaigns;

public interface ICampaignRecipientService
{
    Task<IReadOnlyList<CampaignRecipientCandidateDto>> DiscoverCandidatesAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default);

    Task<CampaignRecipientDto> IncorporateAsync(
        Guid campaignId,
        Guid contactId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignRecipientDto>> ListAsync(Guid campaignId, CancellationToken cancellationToken = default);

    Task<GenerateCampaignDraftsResult> GenerateDraftsAsync(
        Guid campaignId,
        GenerateCampaignDraftsRequest request,
        CancellationToken cancellationToken = default);
}
