using System.Net.Http.Json;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Campaigns;

public sealed class CampaignRecipientApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CampaignRecipientCandidateDto>> DiscoverCandidatesAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/campaigns/{campaignId}/candidates", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<CampaignRecipientCandidateDto>>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignRecipientDto>> ListAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/campaigns/{campaignId}/recipients", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<CampaignRecipientDto>>(response, cancellationToken);
    }

    public async Task<CampaignRecipientDto> IncorporateAsync(
        Guid campaignId,
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync($"api/v1/campaigns/{campaignId}/recipients/{contactId}", null, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignRecipientDto>(response, cancellationToken);
    }

    public async Task<GenerateCampaignDraftsResult> GenerateDraftsAsync(
        Guid campaignId,
        GenerateCampaignDraftsRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            $"api/v1/campaigns/{campaignId}/recipients/generate-drafts",
            request,
            ApiClientJson.Options,
            cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<GenerateCampaignDraftsResult>(response, cancellationToken);
    }
}
