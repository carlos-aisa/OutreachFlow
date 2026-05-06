using System.Net.Http.Json;

using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.Contacts;

public sealed class ContactApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<ContactDto>> ListAsync(
        ContactFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>();

        AddIfNotBlank(query, "search", filter.Search);
        AddIfNotNull(query, "tagId", filter.TagId);
        AddIfNotNull(query, "status", filter.Status);
        AddIfNotNull(query, "doNotContact", filter.DoNotContact);
        AddIfNotNull(query, "organizationId", filter.OrganizationId);
        AddIfNotNull(query, "lastContactedFrom", filter.LastContactedFrom);
        AddIfNotNull(query, "lastContactedTo", filter.LastContactedTo);

        var uri = query.Count == 0
            ? "api/v1/contacts"
            : QueryHelpers.AddQueryString("api/v1/contacts", query);

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ContactDto>>(response, cancellationToken);
    }

    public async Task<ContactDto> CreateAsync(
        CreateContactRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/contacts",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<ContactDto>(response, cancellationToken);
    }

    public async Task<ContactDto> GetByIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/contacts/{contactId}", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<ContactDto>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<ContactActivityDto>> ListActivitiesAsync(
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"api/v1/contacts/{contactId}/activities",
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ContactActivityDto>>(
            response,
            cancellationToken);
    }

    private static void AddIfNotBlank(
        IDictionary<string, string?> query,
        string key,
        string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            query[key] = value.Trim();
        }
    }

    private static void AddIfNotNull<T>(
        IDictionary<string, string?> query,
        string key,
        T? value)
        where T : struct
    {
        if (value is null)
        {
            return;
        }

        query[key] = value.Value switch
        {
            ContactStatus status => status.ToString(),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
            _ => value.Value.ToString()
        };
    }
}
