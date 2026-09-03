using System.Net.Http.Json;

using OutreachFlow.Application.Organizations;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Organizations;

public sealed class OrganizationTypeApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<OrganizationTypeDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/v1/organization-types", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<OrganizationTypeDto>>(response, cancellationToken);
    }

    public async Task<OrganizationTypeDto> CreateAsync(
        CreateOrganizationTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/organization-types",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<OrganizationTypeDto>(response, cancellationToken);
    }

    public async Task<OrganizationTypeDto> UpdateAsync(
        Guid id,
        UpdateOrganizationTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"api/v1/organization-types/{id}",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<OrganizationTypeDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/organization-types/{id}", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }
}
