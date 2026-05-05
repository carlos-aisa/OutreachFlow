using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailMessages;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.SenderProfiles;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class EmailSendingPersistenceTests
{
    [Fact]
    public async Task ShouldPersistEmailMessageAndDraftSendMetadata()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        var senderProfile = new SenderProfile("Primary sender", "sender@example.com");
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var draft = EmailDraft.CreateGenerated(
            contact.Id,
            organizationId: null,
            template.Id,
            senderProfile.Id,
            "Subject",
            "Body",
            hasRenderErrors: false,
            missingVariablesJson: null,
            unknownVariablesJson: null);
        var approvedAt = DateTimeOffset.UtcNow.AddMinutes(-2);
        var sentAt = DateTimeOffset.UtcNow;
        draft.Approve(approvedAt);
        draft.MarkSent(sentAt);
        var emailMessage = EmailMessage.CreateSent(
            contact.Id,
            organizationId: null,
            draft.Id,
            contact.Email,
            draft.Subject,
            draft.Body,
            "Fake",
            "fake-message-id",
            sentAt);

        context.Contacts.Add(contact);
        context.SenderProfiles.Add(senderProfile);
        context.EmailTemplates.Add(template);
        context.EmailDrafts.Add(draft);
        context.EmailMessages.Add(emailMessage);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedDraft = await context.EmailDrafts.SingleAsync();
        var savedMessage = await context.EmailMessages.SingleAsync();

        savedDraft.Status.Should().Be(EmailDraftStatus.Sent);
        savedDraft.SentAt.Should().Be(sentAt);
        savedDraft.FailureReason.Should().BeNull();
        savedMessage.Status.Should().Be(EmailMessageStatus.Sent);
        savedMessage.Provider.Should().Be("Fake");
        savedMessage.ProviderMessageId.Should().Be("fake-message-id");
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
