using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Api;

internal sealed class OutreachFlowApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"outreachflow-tests-{Guid.NewGuid():N}.db");
    private readonly string _attachmentsRootPath = Path.Combine(
        Path.GetTempPath(),
        $"outreachflow-test-attachments-{Guid.NewGuid():N}");

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OutreachFlowDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:OutreachFlow"] = $"Data Source={_databasePath}",
                ["AttachmentStorage:RootPath"] = _attachmentsRootPath,
                ["Logging:LogLevel:Default"] = "Warning",
                ["Logging:LogLevel:Microsoft.EntityFrameworkCore"] = "Warning"
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
