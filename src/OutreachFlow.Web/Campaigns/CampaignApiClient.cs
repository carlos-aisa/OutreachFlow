using System.Net.Http.Json;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Web.Common;
namespace OutreachFlow.Web.Campaigns;
public sealed class CampaignApiClient(HttpClient client)
{
    public async Task<IReadOnlyList<CampaignDto>> ListAsync(CancellationToken cancellationToken = default) { using var response = await client.GetAsync("api/v1/campaigns", cancellationToken); return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<CampaignDto>>(response, cancellationToken); }
    public async Task<CampaignDto> CreateAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default) { using var response = await client.PostAsJsonAsync("api/v1/campaigns", request, ApiClientJson.Options, cancellationToken); return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken); }
    public async Task<CampaignDto> UpdateAsync(Guid id, UpdateCampaignRequest request, CancellationToken cancellationToken = default) { using var response = await client.PutAsJsonAsync($"api/v1/campaigns/{id}", request, ApiClientJson.Options, cancellationToken); return await ApiClientJson.ReadRequiredAsync<CampaignDto>(response, cancellationToken); }
    public async Task<int> GenerateDraftsAsync(Guid id, CancellationToken cancellationToken = default) { using var response = await client.PostAsync($"api/v1/campaigns/{id}/drafts", null, cancellationToken); var result = await ApiClientJson.ReadRequiredAsync<CampaignGenerationResult>(response, cancellationToken); return result.GeneratedDrafts; }
}

public sealed record CampaignGenerationResult(int GeneratedDrafts);
