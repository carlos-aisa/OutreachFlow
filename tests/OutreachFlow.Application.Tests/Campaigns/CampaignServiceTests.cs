using FluentAssertions;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.ContactGroups;
using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Application.Tests.Campaigns;

public sealed class CampaignServiceTests
{
    [Fact]
    public async Task ShouldCreateCampaign()
    {
        var (service, _, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var group = await AddGroupAsync(groupRepository);

        var campaign = await service.CreateAsync(new CreateCampaignRequest(
            "Autumn outreach",
            "Reach new prospects",
            template.Id,
            [group.Id]));

        campaign.Name.Should().Be("Autumn outreach");
        campaign.Status.Should().Be(CampaignStatus.Open);
        campaign.AudienceGroupIds.Should().ContainSingle().Which.Should().Be(group.Id);
    }

    [Fact]
    public async Task ShouldRejectCreateWhenTemplateDoesNotExist()
    {
        var (service, _, _, groupRepository) = CreateService();
        var group = await AddGroupAsync(groupRepository);

        var act = () => service.CreateAsync(new CreateCampaignRequest(
            "Autumn outreach",
            null,
            Guid.NewGuid(),
            [group.Id]));

        await act.Should().ThrowAsync<ApplicationNotFoundException>()
            .WithMessage("Email template was not found.");
    }

    [Fact]
    public async Task ShouldRejectCreateWhenAudienceGroupDoesNotExist()
    {
        var (service, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);

        var act = () => service.CreateAsync(new CreateCampaignRequest(
            "Autumn outreach",
            null,
            template.Id,
            [Guid.NewGuid()]));

        await act.Should().ThrowAsync<ApplicationNotFoundException>()
            .WithMessage("Contact group was not found.");
    }

    [Fact]
    public async Task ShouldRejectCreateWithoutAudienceGroups()
    {
        var (service, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);

        var act = () => service.CreateAsync(new CreateCampaignRequest("Autumn outreach", null, template.Id, []));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Campaign requires at least one audience group.");
    }

    [Fact]
    public async Task ShouldRejectCreateWithoutName()
    {
        var (service, _, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var group = await AddGroupAsync(groupRepository);

        var act = () => service.CreateAsync(new CreateCampaignRequest(" ", null, template.Id, [group.Id]));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Campaign name is required.");
    }

    [Fact]
    public async Task ShouldUpdateCampaign()
    {
        var (service, campaignRepository, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var otherTemplate = await AddTemplateAsync(templateRepository);
        var group = await AddGroupAsync(groupRepository);
        var campaign = await service.CreateAsync(new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id]));

        var updated = await service.UpdateAsync(campaign.Id, new UpdateCampaignRequest("Winter outreach", "Updated", otherTemplate.Id));

        updated.Name.Should().Be("Winter outreach");
        updated.EmailTemplateId.Should().Be(otherTemplate.Id);
        campaignRepository.Campaigns.Should().ContainSingle();
    }

    [Fact]
    public async Task ShouldThrowNotFoundWhenUpdatingUnknownCampaign()
    {
        var (service, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);

        var act = () => service.UpdateAsync(Guid.NewGuid(), new UpdateCampaignRequest("Name", null, template.Id));

        await act.Should().ThrowAsync<ApplicationNotFoundException>()
            .WithMessage("Campaign was not found.");
    }

    [Fact]
    public async Task ShouldAddAndRemoveAudienceGroup()
    {
        var (service, _, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var firstGroup = await AddGroupAsync(groupRepository);
        var secondGroup = await AddGroupAsync(groupRepository);
        var campaign = await service.CreateAsync(new CreateCampaignRequest("Autumn outreach", null, template.Id, [firstGroup.Id]));

        var withSecondGroup = await service.AddAudienceGroupAsync(campaign.Id, secondGroup.Id);
        var withFirstGroupOnly = await service.RemoveAudienceGroupAsync(campaign.Id, secondGroup.Id);

        withSecondGroup.AudienceGroupIds.Should().HaveCount(2);
        withFirstGroupOnly.AudienceGroupIds.Should().ContainSingle().Which.Should().Be(firstGroup.Id);
    }

    [Fact]
    public async Task ShouldRejectRemovingLastAudienceGroup()
    {
        var (service, _, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var group = await AddGroupAsync(groupRepository);
        var campaign = await service.CreateAsync(new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id]));

        var act = () => service.RemoveAudienceGroupAsync(campaign.Id, group.Id);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Campaign requires at least one audience group.");
    }

    [Fact]
    public async Task ShouldCloseAndReopenCampaign()
    {
        var (service, _, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var group = await AddGroupAsync(groupRepository);
        var campaign = await service.CreateAsync(new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id]));

        var closed = await service.CloseAsync(campaign.Id);
        var reopened = await service.ReopenAsync(campaign.Id);

        closed.Status.Should().Be(CampaignStatus.Closed);
        reopened.Status.Should().Be(CampaignStatus.Open);
    }

    [Fact]
    public async Task ShouldListCampaigns()
    {
        var (service, _, templateRepository, groupRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var group = await AddGroupAsync(groupRepository);
        await service.CreateAsync(new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id]));
        await service.CreateAsync(new CreateCampaignRequest("Winter outreach", null, template.Id, [group.Id]));

        var campaigns = await service.ListAsync();

        campaigns.Should().HaveCount(2);
    }

    private static (
        CampaignService Service,
        FakeCampaignRepository CampaignRepository,
        InMemoryEmailTemplateRepository TemplateRepository,
        InMemoryContactGroupRepository GroupRepository) CreateService()
    {
        var campaignRepository = new FakeCampaignRepository();
        var templateRepository = new InMemoryEmailTemplateRepository();
        var groupRepository = new InMemoryContactGroupRepository();
        var service = new CampaignService(campaignRepository, templateRepository, groupRepository, new InMemoryUnitOfWork());
        return (service, campaignRepository, templateRepository, groupRepository);
    }

    private static async Task<EmailTemplate> AddTemplateAsync(InMemoryEmailTemplateRepository repository)
    {
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await repository.AddAsync(template);
        return template;
    }

    private static async Task<ContactGroup> AddGroupAsync(InMemoryContactGroupRepository repository)
    {
        var group = new ContactGroup("Prospects");
        await repository.AddAsync(group);
        return group;
    }

    private sealed class FakeCampaignRepository : ICampaignRepository
    {
        public List<Campaign> Campaigns { get; } = [];

        public Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
        {
            Campaigns.Add(campaign);
            return Task.CompletedTask;
        }

        public Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Campaigns.FirstOrDefault(campaign => campaign.Id == id));
        }

        public Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<Campaign>>(Campaigns);
        }
    }
}
