using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class OutreachFlowDbContextTests
{
    [Fact]
    public async Task ShouldCreateSqliteSchemaWithEnsureCreated()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<OutreachFlowDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new OutreachFlowDbContext(options);

        var created = await context.Database.EnsureCreatedAsync();
        var canConnect = await context.Database.CanConnectAsync();

        created.Should().BeTrue();
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldApplyMigrationsWithMigrate()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");

        try
        {
            var options = new DbContextOptionsBuilder<OutreachFlowDbContext>()
                .UseSqlite($"Data Source={databasePath}")
                .Options;

            await using (var context = new OutreachFlowDbContext(options))
            {
                await context.Database.MigrateAsync();
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();

                appliedMigrations.Any(m => m.EndsWith("_InitialCreate", StringComparison.Ordinal))
                    .Should()
                    .BeTrue();
            }
        }
        finally
        {
            SqliteConnection.ClearAllPools();

            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }
        }
    }
}
