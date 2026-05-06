using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class FollowUpTaskEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldCreateListAndCompleteFollowUpTask()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var contact = await PostAsync<ContactDto>(
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

        var task = await PostAsync<FollowUpTaskDto>(
            client,
            "/api/v1/follow-ups",
            new CreateFollowUpTaskRequest(
                contact.Id,
                null,
                DateTimeOffset.UtcNow.AddDays(2),
                FollowUpTaskType.Email,
                "Check in next week."));

        var pending = await GetAsync<IReadOnlyList<FollowUpTaskDto>>(
            client,
            "/api/v1/follow-ups?isCompleted=false");
        pending.Should().ContainSingle(item => item.Id == task.Id);

        var completed = await PostAsync<FollowUpTaskDto>(
            client,
            $"/api/v1/follow-ups/{task.Id}/complete",
            request: null);
        completed.IsCompleted.Should().BeTrue();

        var completedTasks = await GetAsync<IReadOnlyList<FollowUpTaskDto>>(
            client,
            "/api/v1/follow-ups?isCompleted=true");
        completedTasks.Should().ContainSingle(item => item.Id == task.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownFollowUpTask()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/v1/follow-ups/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<T> GetAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string uri, object? request)
    {
        HttpResponseMessage response;

        if (request is null)
        {
            response = await client.PostAsync(uri, null);
        }
        else
        {
            response = await client.PostAsJsonAsync(uri, request, JsonOptions);
        }

        using (response)
        {
            response.EnsureSuccessStatusCode();
            return await ReadAsync<T>(response);
        }
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
}
