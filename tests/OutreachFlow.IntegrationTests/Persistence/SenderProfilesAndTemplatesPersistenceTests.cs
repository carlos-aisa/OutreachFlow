using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.SenderProfiles;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class SenderProfilesAndTemplatesPersistenceTests
{
    [Fact]
    public async Task ShouldPersistSenderProfileAndEmailTemplate()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var senderProfile = new SenderProfile(
            "Primary sender",
            "sender@example.com",
            organizationName: "Northwind Studio",
            signature: "Best regards",
            isDefault: true);
        var template = new EmailTemplate(
            "Intro",
            "Initial outreach",
            "Hello {{contact.displayName}}",
            "Hello {{contact.displayName}},\n\n{{sender.signature}}");

        context.SenderProfiles.Add(senderProfile);
        context.EmailTemplates.Add(template);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedSenderProfile = await context.SenderProfiles.SingleAsync();
        var savedTemplate = await context.EmailTemplates.SingleAsync();
        savedSenderProfile.IsDefault.Should().BeTrue();
        savedSenderProfile.IsActive.Should().BeTrue();
        savedTemplate.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldFilterActiveEmailTemplates()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var activeTemplate = new EmailTemplate("Active", null, "Subject", "Body");
        var inactiveTemplate = new EmailTemplate("Inactive", null, "Subject", "Body");
        inactiveTemplate.Deactivate(DateTimeOffset.UtcNow);
        context.EmailTemplates.AddRange(activeTemplate, inactiveTemplate);
        await context.SaveChangesAsync();

        var activeTemplates = await context.EmailTemplates
            .Where(template => template.IsActive)
            .ToArrayAsync();

        activeTemplates.Should().ContainSingle()
            .Which.Name.Should().Be("Active");
    }

    private static async Task<SqliteConnection> OpenConnectionAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        return connection;
    }

    private static async Task<OutreachFlowDbContext> CreateMigratedContextAsync(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<OutreachFlowDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new OutreachFlowDbContext(options);
        await context.Database.MigrateAsync();
        return context;
    }
}
