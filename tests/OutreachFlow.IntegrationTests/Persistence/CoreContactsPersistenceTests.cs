using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.Tags;
using OutreachFlow.Infrastructure.Persistence;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class CoreContactsPersistenceTests
{
    [Fact]
    public async Task ShouldPersistOrganizationContactTagAndAssignment()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var organization = new Organization("Northwind Studio");
        var tag = new Tag("Priority", "Workflow");
        var contact = new Contact(
            "Alex Morgan",
            "alex@example.com",
            organization.Id,
            role: "Operations Manager",
            status: ContactStatus.New);
        contact.AssignTag(tag.Id);

        context.Organizations.Add(organization);
        context.Tags.Add(tag);
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedContact = await context.Contacts
            .Include(saved => saved.Tags)
            .SingleAsync(saved => saved.Email == "alex@example.com");

        savedContact.OrganizationId.Should().Be(organization.Id);
        savedContact.Tags.Should().ContainSingle()
            .Which.TagId.Should().Be(tag.Id);
    }

    [Fact]
    public async Task ShouldRejectDuplicateNormalizedContactEmail()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        context.Contacts.Add(new Contact("Alex Morgan", "alex@example.com"));
        await context.SaveChangesAsync();

        context.Contacts.Add(new Contact("Avery Lee", " ALEX@example.com "));

        var act = () => context.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task ShouldRejectDuplicateTagNameWithinSameCategory()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        context.Tags.Add(new Tag("Priority", "Workflow"));
        await context.SaveChangesAsync();

        context.Tags.Add(new Tag(" priority ", " workflow "));

        var act = () => context.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task ShouldRejectContactWithMissingOrganization()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        context.Contacts.Add(new Contact(
            "Alex Morgan",
            "alex@example.com",
            Guid.NewGuid()));

        var act = () => context.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
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
