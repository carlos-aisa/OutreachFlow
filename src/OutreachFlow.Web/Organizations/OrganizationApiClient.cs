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
}
