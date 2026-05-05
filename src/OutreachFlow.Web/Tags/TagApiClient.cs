using System.Net.Http.Json;

using OutreachFlow.Application.Tags;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Tags;

public sealed class TagApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<TagDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/v1/tags", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<TagDto>>(response, cancellationToken);
    }

    public async Task<TagDto> CreateAsync(
        CreateTagRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/tags",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<TagDto>(response, cancellationToken);
    }
}
