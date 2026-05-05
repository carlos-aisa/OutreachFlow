using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.EmailDrafts;

public sealed class EmailDraftApiClient(HttpClient httpClient)
{
    public async Task<GenerateEmailDraftsResult> GenerateAsync(
        GenerateEmailDraftsRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/drafts/generate",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<GenerateEmailDraftsResult>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<EmailDraftDto>> ListAsync(
        EmailDraftStatus? status = null,
        Guid? contactId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>();

        if (status is not null)
        {
            query["status"] = status.Value.ToString();
        }

        if (contactId is not null)
        {
            query["contactId"] = contactId.Value.ToString();
        }

        var uri = query.Count == 0
            ? "api/v1/drafts"
            : QueryHelpers.AddQueryString("api/v1/drafts", query);

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<EmailDraftDto>>(response, cancellationToken);
    }
}
