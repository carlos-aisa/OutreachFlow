using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Web.Contacts;

namespace OutreachFlow.IntegrationTests.Web;

public sealed class ContactApiClientTests
{
    [Fact]
    public async Task ShouldSendPostRequestWhenAssigningTag()
    {
        var contactId = Guid.NewGuid();
        var tagId = Guid.NewGuid();
        var handler = new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NoContent));
        using var httpClient = CreateHttpClient(handler);
        var client = new ContactApiClient(httpClient);

        await client.AssignTagAsync(contactId, tagId);

        handler.Requests.Should().ContainSingle();
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].PathAndQuery.Should().Be($"/api/v1/contacts/{contactId}/tags/{tagId}");
    }

    [Fact]
    public async Task ShouldSendDeleteRequestWhenRemovingTag()
    {
        var contactId = Guid.NewGuid();
        var tagId = Guid.NewGuid();
        var handler = new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NoContent));
        using var httpClient = CreateHttpClient(handler);
        var client = new ContactApiClient(httpClient);

        await client.RemoveTagAsync(contactId, tagId);

        handler.Requests.Should().ContainSingle();
        handler.Requests[0].Method.Should().Be(HttpMethod.Delete);
        handler.Requests[0].PathAndQuery.Should().Be($"/api/v1/contacts/{contactId}/tags/{tagId}");
    }

    [Fact]
    public async Task ShouldSendPostRequestWhenCreatingContactIntake()
    {
        var handler = new RecordingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ContactDto(
                Guid.NewGuid(),
                null,
                null,
                "Alex Morgan",
                "alex@example.com",
                null,
                null,
                null,
                ContactStatus.New,
                false,
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                []))
        });
        using var httpClient = CreateHttpClient(handler);
        var client = new ContactApiClient(httpClient);

        await client.CreateIntakeAsync(new CreateContactIntakeRequest(
            null,
            null,
            "Alex Morgan",
            "alex@example.com",
            null,
            null,
            null,
            ContactStatus.New,
            false));

        handler.Requests.Should().ContainSingle();
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].PathAndQuery.Should().Be("/api/v1/contacts/intakes");
    }

    private static HttpClient CreateHttpClient(HttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    private sealed class RecordingHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add((request.Method, request.RequestUri?.PathAndQuery ?? string.Empty));
            return Task.FromResult(response);
        }
    }
}
