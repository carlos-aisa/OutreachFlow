using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Infrastructure.Persistence;
using OutreachFlow.Infrastructure.Persistence.Repositories;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class ContactActivitiesPersistenceTests
{
    [Fact]
    public async Task ShouldPersistAndListActivitiesByContactInDescendingOrder()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var repository = new ContactActivityRepository(context);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();
        var firstOccurredAt = DateTimeOffset.UtcNow.AddMinutes(-20);
        var secondOccurredAt = DateTimeOffset.UtcNow.AddMinutes(-10);

        await repository.AddAsync(ContactActivity.Create(
            contact.Id,
            null,
            ContactActivityType.ContactCreated,
            "Contact created",
            null,
            null,
            firstOccurredAt));
        await repository.AddAsync(ContactActivity.Create(
            contact.Id,
            null,
            ContactActivityType.ContactUpdated,
            "Contact updated",
            null,
            null,
            secondOccurredAt));
        await context.SaveChangesAsync();

        var activities = await repository.ListByContactIdAsync(contact.Id);

        activities.Should().HaveCount(2);
        activities[0].Type.Should().Be(ContactActivityType.ContactUpdated);
        activities[1].Type.Should().Be(ContactActivityType.ContactCreated);
    }

    [Fact]
    public async Task ShouldFilterActivitiesByContactId()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var repository = new ContactActivityRepository(context);
        var matchingContact = new Contact("Alex Morgan", "alex@example.com");
        var otherContact = new Contact("Sam Taylor", "sam@example.com");
        context.Contacts.AddRange(matchingContact, otherContact);
        await context.SaveChangesAsync();

        await repository.AddAsync(ContactActivity.Create(
            matchingContact.Id,
            null,
            ContactActivityType.ContactCreated,
            "Matching contact created",
            null,
            null,
            DateTimeOffset.UtcNow.AddMinutes(-5)));
        await repository.AddAsync(ContactActivity.Create(
            otherContact.Id,
            null,
            ContactActivityType.ContactCreated,
            "Other contact created",
            null,
            null,
            DateTimeOffset.UtcNow));
        await context.SaveChangesAsync();

        var activities = await repository.ListByContactIdAsync(matchingContact.Id);

        activities.Should().ContainSingle();
        activities[0].ContactId.Should().Be(matchingContact.Id);
        activities[0].Subject.Should().Be("Matching contact created");
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
