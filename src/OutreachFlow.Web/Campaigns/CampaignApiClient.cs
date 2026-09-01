using System.Net.Http.Json;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Campaigns;

public sealed class CampaignApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<CampaignDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/v1/campaigns", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<CampaignDto>>(response, cancellationToken);
    }

    public async Task<CampaignDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/campaigns/{id}", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }

    public async Task<CampaignDto> CreateAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/v1/campaigns", request, ApiClientJson.Options, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }

    public async Task<CampaignDto> UpdateAsync(Guid id, UpdateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"api/v1/campaigns/{id}", request, ApiClientJson.Options, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }

    public async Task<CampaignDto> AddAudienceGroupAsync(Guid id, Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync($"api/v1/campaigns/{id}/audience-groups/{contactGroupId}", null, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }

    public async Task<CampaignDto> RemoveAudienceGroupAsync(Guid id, Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/campaigns/{id}/audience-groups/{contactGroupId}", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }

    public async Task<CampaignDto> CloseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync($"api/v1/campaigns/{id}/close", null, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }

    public async Task<CampaignDto> ReopenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync($"api/v1/campaigns/{id}/reopen", null, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken);
    }
}
