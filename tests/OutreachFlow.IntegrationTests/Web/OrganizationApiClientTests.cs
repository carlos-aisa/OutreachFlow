using System.Net;
using System.Text;
using FluentAssertions;
using OutreachFlow.Web.Organizations;

namespace OutreachFlow.IntegrationTests.Web;

public sealed class OrganizationApiClientTests
{
    [Fact]
    public async Task ShouldThrowFriendlyErrorWhenApiReturnsNonJsonError()
    {
        using var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(
                "Microsoft.Data.Sqlite.SqliteException: SQLite Error 1: no such table: Organizations.",
                Encoding.UTF8,
                "text/plain")
        });
        var client = new OrganizationApiClient(httpClient);

        var act = () => client.ListAsync();

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The API request failed with status code 500 (Internal Server Error). The response was not a valid OutreachFlow error payload. Check the API logs for details.");
    }

    [Fact]
    public async Task ShouldThrowFriendlyErrorWhenApiReturnsInvalidJsonSuccessPayload()
    {
        using var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Moved Permanently", Encoding.UTF8, "text/plain")
        });
        var client = new OrganizationApiClient(httpClient);

        var act = () => client.ListAsync();

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("The API returned a response that was not valid JSON.");
    }

    private static HttpClient CreateHttpClient(HttpResponseMessage response)
    {
        return new HttpClient(new StubHttpMessageHandler(response))
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    private sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(response);
        }
    }
}
