using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.ContactImports;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.ContactImports;

public sealed class ContactImportApiClient(HttpClient httpClient)
{
    public async Task<ContactImportPreviewResult> PreviewAsync(
        ContactImportPreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/contact-imports/preview",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<ContactImportPreviewResult>(response, cancellationToken);
    }

    public async Task<ContactImportCommitResult> CommitAsync(
        ContactImportCommitRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/contact-imports/commit",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<ContactImportCommitResult>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<ImportJobDto>> ListJobsAsync(
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var uri = limit is int value && value > 0
            ? QueryHelpers.AddQueryString(
                "api/v1/contact-imports/jobs",
                "limit",
                value.ToString(CultureInfo.InvariantCulture))
            : "api/v1/contact-imports/jobs";

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ImportJobDto>>(response, cancellationToken);
    }
}
