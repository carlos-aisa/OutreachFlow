using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.SenderProfiles;

public sealed class SenderProfileApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<SenderProfileDto>> ListAsync(
        bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var uri = activeOnly is null
            ? "api/v1/sender-profiles"
            : QueryHelpers.AddQueryString("api/v1/sender-profiles", "activeOnly", activeOnly.Value.ToString());

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<SenderProfileDto>>(response, cancellationToken);
    }

    public async Task<SenderProfileDto> CreateAsync(
        CreateSenderProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/sender-profiles",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<SenderProfileDto>(response, cancellationToken);
    }

    public async Task<SenderProfileDto> UpdateAsync(
        Guid id,
        UpdateSenderProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"api/v1/sender-profiles/{id}",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<SenderProfileDto>(response, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/sender-profiles/{id}", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }
}
