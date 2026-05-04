using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

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

    private sealed class OutreachFlowApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            $"outreachflow-tests-{Guid.NewGuid():N}.db");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                var overrides = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:OutreachFlow"] = $"Data Source={_databasePath}"
                };

                configurationBuilder.AddInMemoryCollection(overrides);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
    }
}
