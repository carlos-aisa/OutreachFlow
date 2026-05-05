using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class HealthEndpointTests
{
    [Fact]
    public async Task GetHealthShouldReturnHealthyStatus()
    {
        using var factory = new OutreachFlowApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(payload);

        document.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
        document.RootElement.TryGetProperty("utcNow", out var utcNowProperty).Should().BeTrue();
        utcNowProperty.GetString().Should().NotBeNullOrWhiteSpace();
    }
}
