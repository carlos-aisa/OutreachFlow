using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class ContactActivitiesEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldReturnContactActivityHistoryInDescendingOccurredAtOrder()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        var createdContact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                null,
                "Alex Morgan",
                "alex@example.com",
                null,
                null,
                null,
                ContactStatus.New,
                false));

        _ = await PutAsync<ContactDto>(
            client,
            $"/api/v1/contacts/{createdContact.Id}",
            new UpdateContactRequest(
                null,
                "Alex Morgan",
                "alex@example.com",
                null,
                null,
                null,
                ContactStatus.Contacted,
                false));

        var activities = await GetAsync<IReadOnlyList<ContactActivityDto>>(
            client,
            $"/api/v1/contacts/{createdContact.Id}/activities");

        activities.Should().NotBeEmpty();
        activities.Should().Contain(activity => activity.Type == ContactActivityType.ContactCreated);
        activities.Should().Contain(activity => activity.Type == ContactActivityType.ContactUpdated);
        activities.Should().Contain(activity => activity.Type == ContactActivityType.StatusChanged);
        activities.Should().BeInDescendingOrder(activity => activity.OccurredAt);
    }

    [Fact]
    public async Task ShouldReturnNotFoundWhenContactDoesNotExist()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/v1/contacts/{Guid.NewGuid()}/activities");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var error = await ReadAsync<ApiErrorResponse>(response);
        error.ErrorCode.Should().Be("NOT_FOUND");
    }

    private static async Task<T> GetAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string uri, object request)
    {
        using var response = await client.PostAsJsonAsync(uri, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PutAsync<T>(HttpClient client, string uri, object request)
    {
        using var response = await client.PutAsJsonAsync(uri, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return result ?? throw new InvalidOperationException("The API returned an empty response.");
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private sealed record ApiErrorResponse(string ErrorCode, string Message);
}
