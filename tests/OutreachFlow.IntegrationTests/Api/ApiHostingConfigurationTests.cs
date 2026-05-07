using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class ApiHostingConfigurationTests
{
    [Fact]
    public async Task ShouldServeHttpWhenHttpsRedirectionIsDisabledByConfiguration()
    {
        using var factory = new ApiHostingConfigurationFactory();
        await factory.InitializeDatabaseAsync();

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost")
        });

        var response = await client.GetAsync("/api/v1/health");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Headers.Location.Should().BeNull();
    }

    private sealed class ApiHostingConfigurationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(
            Path.GetTempPath(),
            $"outreachflow-hosting-tests-{Guid.NewGuid():N}.db");

        private readonly string _attachmentsRootPath = Path.Combine(
            Path.GetTempPath(),
            $"outreachflow-hosting-attachments-{Guid.NewGuid():N}");

        public async Task InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OutreachFlowDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                var overrides = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:OutreachFlow"] = $"Data Source={_databasePath}",
                    ["AttachmentStorage:RootPath"] = _attachmentsRootPath,
                    ["EmailSending:Provider"] = "Fake",
                    ["Hosting:UseHttpsRedirection"] = "false"
                };

                configurationBuilder.AddInMemoryCollection(overrides);
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<OutreachFlowDbContext>();
                services.RemoveAll<DbContextOptions<OutreachFlowDbContext>>();
                services.AddDbContext<OutreachFlowDbContext>(options =>
                    options.UseSqlite($"Data Source={_databasePath}"));
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            SqliteConnection.ClearAllPools();

            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }

            if (Directory.Exists(_attachmentsRootPath))
            {
                Directory.Delete(_attachmentsRootPath, recursive: true);
            }
        }
    }
}
