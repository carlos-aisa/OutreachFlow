using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class EmailDraftGenerationPersistenceTests
{
    [Fact]
    public async Task ShouldPersistGeneratedEmailDraftWithAttachmentsAndDiagnostics()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var organization = new Organization("Northwind");
        var contact = new Contact(
            "Alex Morgan",
            "alex@example.com",
            organizationId: organization.Id);
        var senderProfile = new SenderProfile("Primary sender", "sender@example.com");
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var attachment = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "storage/brochure.pdf",
            1024);
        var draft = EmailDraft.CreateGenerated(
            contact.Id,
            organization.Id,
            template.Id,
            senderProfile.Id,
            "Subject",
            "Body",
            hasRenderErrors: true,
            missingVariablesJson: "[\"organization.name\"]",
            unknownVariablesJson: "[]");
        draft.AssignAttachment(attachment, DateTimeOffset.UtcNow);

        context.Organizations.Add(organization);
        context.Contacts.Add(contact);
        context.SenderProfiles.Add(senderProfile);
        context.EmailTemplates.Add(template);
        context.AttachmentAssets.Add(attachment);
        context.EmailDrafts.Add(draft);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedDraft = await context.EmailDrafts
            .Include(item => item.Attachments)
            .SingleAsync();

        savedDraft.Status.Should().Be(EmailDraftStatus.NeedsReview);
        savedDraft.MissingVariablesJson.Should().Be("[\"organization.name\"]");
        savedDraft.Attachments.Should().ContainSingle(item => item.AttachmentAssetId == attachment.Id);
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
