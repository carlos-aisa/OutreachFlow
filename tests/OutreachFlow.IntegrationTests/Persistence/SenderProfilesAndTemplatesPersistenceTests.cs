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
            signature: "<p>Best regards</p>",
            signatureFormat: SenderSignatureFormat.Html,
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
        savedSenderProfile.SignatureFormat.Should().Be(SenderSignatureFormat.Html);
        savedTemplate.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldPersistHtmlAndRtfSignatures()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);

        context.SenderProfiles.Add(new SenderProfile(
            "Html sender",
            "html.sender@example.com",
            signature: "<p>Best regards</p>",
            signatureFormat: SenderSignatureFormat.Html));
        context.SenderProfiles.Add(new SenderProfile(
            "Rtf sender",
            "rtf.sender@example.com",
            signature: @"{\rtf1\ansi Best regards}",
            signatureFormat: SenderSignatureFormat.Rtf));
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var senderProfiles = await context.SenderProfiles
            .OrderBy(senderProfile => senderProfile.Email)
            .ToArrayAsync();

        senderProfiles.Should().HaveCount(2);
        senderProfiles.Single(item => item.Email == "html.sender@example.com")
            .SignatureFormat.Should().Be(SenderSignatureFormat.Html);
        senderProfiles.Single(item => item.Email == "rtf.sender@example.com")
            .SignatureFormat.Should().Be(SenderSignatureFormat.Rtf);
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
