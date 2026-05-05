using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class AttachmentAssetsPersistenceTests
{
    [Fact]
    public async Task ShouldPersistAttachmentAssetAndTemplateDefaultAttachment()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var attachmentAsset = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "storage/2026/05/brochure.pdf",
            2048);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        template.AssignDefaultAttachment(attachmentAsset, DateTimeOffset.UtcNow);
        context.AttachmentAssets.Add(attachmentAsset);
        context.EmailTemplates.Add(template);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedAttachment = await context.AttachmentAssets.SingleAsync();
        var savedTemplate = await context.EmailTemplates
            .Include(item => item.DefaultAttachments)
            .SingleAsync();

        savedAttachment.Name.Should().Be("Brochure");
        savedTemplate.DefaultAttachments.Should().ContainSingle(defaultAttachment =>
            defaultAttachment.AttachmentAssetId == savedAttachment.Id);
    }

    [Fact]
    public async Task ShouldFilterActiveAttachmentAssets()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var activeAttachment = new AttachmentAsset(
            "Active brochure",
            "active.pdf",
            "application/pdf",
            "storage/active.pdf",
            1024);
        var inactiveAttachment = new AttachmentAsset(
            "Inactive brochure",
            "inactive.pdf",
            "application/pdf",
            "storage/inactive.pdf",
            1024);
        inactiveAttachment.Deactivate();
        context.AttachmentAssets.AddRange(activeAttachment, inactiveAttachment);
        await context.SaveChangesAsync();

        var activeAttachments = await context.AttachmentAssets
            .Where(attachmentAsset => attachmentAsset.IsActive)
            .ToArrayAsync();

        activeAttachments.Should().ContainSingle()
            .Which.Name.Should().Be("Active brochure");
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
