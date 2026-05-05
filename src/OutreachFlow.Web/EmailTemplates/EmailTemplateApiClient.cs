using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Templates;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.EmailTemplates;

public sealed class EmailTemplateApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<EmailTemplateDto>> ListAsync(
        bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var uri = activeOnly is null
            ? "api/v1/templates"
            : QueryHelpers.AddQueryString("api/v1/templates", "activeOnly", activeOnly.Value.ToString());

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<EmailTemplateDto>>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<TemplateVariableDto>> ListVariablesAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/v1/templates/variables", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<TemplateVariableDto>>(response, cancellationToken);
    }

    public async Task<EmailTemplateDto> CreateAsync(
        CreateEmailTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/templates",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<EmailTemplateDto>(response, cancellationToken);
    }

    public async Task<EmailTemplateDto> UpdateAsync(
        Guid id,
        UpdateEmailTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"api/v1/templates/{id}",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<EmailTemplateDto>(response, cancellationToken);
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/templates/{id}", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }
}
