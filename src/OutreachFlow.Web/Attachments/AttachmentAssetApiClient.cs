using System.Net.Http.Headers;
using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Attachments;

public sealed class AttachmentAssetApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<AttachmentAssetDto>> ListAsync(
        bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var uri = activeOnly is null
            ? "api/v1/attachments"
            : QueryHelpers.AddQueryString("api/v1/attachments", "activeOnly", activeOnly.Value.ToString());

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<AttachmentAssetDto>>(response, cancellationToken);
    }

    public async Task<AttachmentAssetDto> UploadAsync(
        string name,
        string? description,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(name), "name");

        if (!string.IsNullOrWhiteSpace(description))
        {
            formData.Add(new StringContent(description), "description");
        }

        using var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
        formData.Add(fileContent, "file", fileName);

        using var response = await httpClient.PostAsync("api/v1/attachments", formData, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<AttachmentAssetDto>(response, cancellationToken);
    }

    public async Task<AttachmentAssetDto> UpdateAsync(
        Guid id,
        UpdateAttachmentAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"api/v1/attachments/{id}",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<AttachmentAssetDto>(response, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/attachments/{id}", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }
}
