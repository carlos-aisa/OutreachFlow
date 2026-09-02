using System.Net.Http.Json;

using OutreachFlow.Application.Organizations;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Organizations;

public sealed class OrganizationApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/v1/organizations", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<OrganizationDto>>(response, cancellationToken);
    }

    public async Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/organizations",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<OrganizationDto>(response, cancellationToken);
    }

    public async Task<OrganizationDto> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"api/v1/organizations/{id}",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<OrganizationDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/organizations/{id}", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }
}
