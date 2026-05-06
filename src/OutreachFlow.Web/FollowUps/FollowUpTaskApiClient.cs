using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.FollowUps;

public sealed class FollowUpTaskApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<FollowUpTaskDto>> ListAsync(
        FollowUpTaskFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>();

        if (filter.ContactId is not null)
        {
            query["contactId"] = filter.ContactId.Value.ToString();
        }

        if (filter.IsCompleted is not null)
        {
            query["isCompleted"] = filter.IsCompleted.Value.ToString().ToLowerInvariant();
        }

        if (filter.DueFrom is not null)
        {
            query["dueFrom"] = filter.DueFrom.Value.ToString("O");
        }

        if (filter.DueTo is not null)
        {
            query["dueTo"] = filter.DueTo.Value.ToString("O");
        }

        if (filter.Limit is int limit && limit > 0)
        {
            query["limit"] = limit.ToString(CultureInfo.InvariantCulture);
        }

        var uri = query.Count == 0
            ? "api/v1/follow-ups"
            : QueryHelpers.AddQueryString("api/v1/follow-ups", query);

        using var response = await httpClient.GetAsync(uri, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<FollowUpTaskDto>>(response, cancellationToken);
    }

    public async Task<FollowUpTaskDto> CreateAsync(
        CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/v1/follow-ups",
            request,
            ApiClientJson.Options,
            cancellationToken);

        return await ApiClientJson.ReadRequiredAsync<FollowUpTaskDto>(response, cancellationToken);
    }

    public async Task<FollowUpTaskDto> CompleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync($"api/v1/follow-ups/{id}/complete", null, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<FollowUpTaskDto>(response, cancellationToken);
    }
}
