using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.ContactGroups;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Infrastructure.Persistence;
using OutreachFlow.Infrastructure.Persistence.Repositories;

namespace OutreachFlow.IntegrationTests.Persistence;

public sealed class CampaignPersistenceTests
{
    [Fact]
    public async Task ShouldPersistAndReloadCampaignWithAudienceGroups()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var firstGroup = new ContactGroup("Prospects");
        var secondGroup = new ContactGroup("Leads");
        context.EmailTemplates.Add(template);
        context.ContactGroups.Add(firstGroup);
        context.ContactGroups.Add(secondGroup);
        await context.SaveChangesAsync();
        var repository = new CampaignRepository(context);
        var campaign = new Campaign("Autumn outreach", "Reach new prospects", template.Id, [firstGroup.Id, secondGroup.Id]);

        await repository.AddAsync(campaign);
        await context.SaveChangesAsync();

        var reloaded = await repository.GetByIdAsync(campaign.Id);

        reloaded.Should().NotBeNull();
        reloaded!.Name.Should().Be("Autumn outreach");
        reloaded.AudienceGroups.Should().HaveCount(2);
        reloaded.AudienceGroups.Select(audienceGroup => audienceGroup.ContactGroupId)
            .Should().BeEquivalentTo([firstGroup.Id, secondGroup.Id]);
    }

    [Fact]
    public async Task ShouldListCampaignsOrderedByName()
    {
        await using var connection = await OpenConnectionAsync();
        await using var context = await CreateMigratedContextAsync(connection);
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var group = new ContactGroup("Prospects");
        context.EmailTemplates.Add(template);
        context.ContactGroups.Add(group);
        await context.SaveChangesAsync();
        var repository = new CampaignRepository(context);
        await repository.AddAsync(new Campaign("Winter outreach", null, template.Id, [group.Id]));
        await repository.AddAsync(new Campaign("Autumn outreach", null, template.Id, [group.Id]));
        await context.SaveChangesAsync();

        var campaigns = await repository.ListAsync();

        campaigns.Select(campaign => campaign.Name).Should().Equal("Autumn outreach", "Winter outreach");
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
