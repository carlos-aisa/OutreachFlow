using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.FollowUps;
using OutreachFlow.Infrastructure.Persistence;
using OutreachFlow.Infrastructure.Persistence.Repositories;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class FollowUpTaskPersistenceTests
{
    [Fact]
    public async Task ShouldPersistAndListFollowUpTasksByDueAt()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var repository = new FollowUpTaskRepository(context);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        await repository.AddAsync(new FollowUpTask(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(3),
            FollowUpTaskType.Email,
            "Later"));
        await repository.AddAsync(new FollowUpTask(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(1),
            FollowUpTaskType.Call,
            "Sooner"));
        await context.SaveChangesAsync();

        var pending = await repository.ListAsync(new FollowUpTaskFilterRequest(
            ContactId: contact.Id,
            IsCompleted: false,
            DueFrom: null,
            DueTo: null));

        pending.Should().HaveCount(2);
        pending[0].Notes.Should().Be("Sooner");
        pending[1].Notes.Should().Be("Later");
    }

    [Fact]
    public async Task ShouldFilterCompletedFollowUpTasks()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var repository = new FollowUpTaskRepository(context);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();
        var pendingTask = new FollowUpTask(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(1),
            FollowUpTaskType.Email);
        var completedTask = new FollowUpTask(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(2),
            FollowUpTaskType.Call);
        completedTask.Complete(DateTimeOffset.UtcNow);
        await repository.AddAsync(pendingTask);
        await repository.AddAsync(completedTask);
        await context.SaveChangesAsync();

        var completed = await repository.ListAsync(new FollowUpTaskFilterRequest(
            ContactId: contact.Id,
            IsCompleted: true,
            DueFrom: null,
            DueTo: null));

        completed.Should().ContainSingle();
        completed[0].Type.Should().Be(FollowUpTaskType.Call);
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
