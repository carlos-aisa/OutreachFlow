using FluentAssertions;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Domain.Tests.Campaigns;

public sealed class CampaignTests
{
    [Fact]
    public void ShouldCreateOpenCampaignWithAudienceGroups()
    {
        var templateId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        var campaign = new Campaign("Autumn outreach", "Reach new prospects", templateId, [groupId]);

        campaign.Id.Should().NotBeEmpty();
        campaign.Name.Should().Be("Autumn outreach");
        campaign.Description.Should().Be("Reach new prospects");
        campaign.EmailTemplateId.Should().Be(templateId);
        campaign.Status.Should().Be(CampaignStatus.Open);
        campaign.AudienceGroups.Should().ContainSingle(audienceGroup => audienceGroup.ContactGroupId == groupId);
    }

    [Fact]
    public void ShouldRejectCampaignWithoutName()
    {
        var act = () => new Campaign(" ", null, Guid.NewGuid(), [Guid.NewGuid()]);

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign name is required.");
    }

    [Fact]
    public void ShouldRejectCampaignWithoutTemplate()
    {
        var act = () => new Campaign("Autumn outreach", null, Guid.Empty, [Guid.NewGuid()]);

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign message is required.");
    }

    [Fact]
    public void ShouldRejectCampaignWithoutAudienceGroups()
    {
        var act = () => new Campaign("Autumn outreach", null, Guid.NewGuid(), []);

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign requires at least one audience group.");
    }

    [Fact]
    public void ShouldDeduplicateAudienceGroupIds()
    {
        var groupId = Guid.NewGuid();

        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [groupId, groupId]);

        campaign.AudienceGroups.Should().ContainSingle();
    }

    [Fact]
    public void ShouldRename()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);

        campaign.Rename("Winter outreach", "Updated purpose");

        campaign.Name.Should().Be("Winter outreach");
        campaign.Description.Should().Be("Updated purpose");
    }

    [Fact]
    public void ShouldAddAudienceGroup()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);
        var newGroupId = Guid.NewGuid();

        var added = campaign.AddAudienceGroup(newGroupId);

        added.Should().BeTrue();
        campaign.AudienceGroups.Should().Contain(audienceGroup => audienceGroup.ContactGroupId == newGroupId);
    }

    [Fact]
    public void ShouldNotAddDuplicateAudienceGroup()
    {
        var groupId = Guid.NewGuid();
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [groupId]);

        var added = campaign.AddAudienceGroup(groupId);

        added.Should().BeFalse();
        campaign.AudienceGroups.Should().ContainSingle();
    }

    [Fact]
    public void ShouldRemoveAudienceGroupWhenMoreThanOneRemains()
    {
        var firstGroupId = Guid.NewGuid();
        var secondGroupId = Guid.NewGuid();
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [firstGroupId, secondGroupId]);

        var removed = campaign.RemoveAudienceGroup(firstGroupId);

        removed.Should().BeTrue();
        campaign.AudienceGroups.Should().ContainSingle(audienceGroup => audienceGroup.ContactGroupId == secondGroupId);
    }

    [Fact]
    public void ShouldRejectRemovingLastAudienceGroup()
    {
        var groupId = Guid.NewGuid();
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [groupId]);

        var act = () => campaign.RemoveAudienceGroup(groupId);

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign requires at least one audience group.");
    }

    [Fact]
    public void ShouldChangeMessageWhileOpen()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);
        var newTemplateId = Guid.NewGuid();

        campaign.ChangeMessage(newTemplateId);

        campaign.EmailTemplateId.Should().Be(newTemplateId);
    }

    [Fact]
    public void ShouldRejectChangesWhileClosed()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);
        campaign.Close();

        var act = () => campaign.ChangeMessage(Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Only open campaigns can be changed.");
    }

    [Fact]
    public void ShouldRejectAddingAudienceGroupWhileClosed()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);
        campaign.Close();

        var act = () => campaign.AddAudienceGroup(Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Only open campaigns can be changed.");
    }

    [Fact]
    public void ShouldCloseOpenCampaign()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);

        campaign.Close();

        campaign.Status.Should().Be(CampaignStatus.Closed);
    }

    [Fact]
    public void ShouldRejectClosingAlreadyClosedCampaign()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);
        campaign.Close();

        var act = () => campaign.Close();

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign is already closed.");
    }

    [Fact]
    public void ShouldReopenClosedCampaign()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);
        campaign.Close();

        campaign.Reopen();

        campaign.Status.Should().Be(CampaignStatus.Open);
    }

    [Fact]
    public void ShouldRejectReopeningAlreadyOpenCampaign()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);

        var act = () => campaign.Reopen();

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign is already open.");
    }

    [Fact]
    public void ShouldDefaultFollowUpToDisabled()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);

        campaign.FollowUpEnabled.Should().BeFalse();
        campaign.FollowUpDueDays.Should().Be(7);
        campaign.FollowUpType.Should().Be(FollowUpTaskType.Email);
    }

    [Fact]
    public void ShouldConfigureFollowUp()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);

        campaign.ConfigureFollowUp(true, 5, FollowUpTaskType.Call);

        campaign.FollowUpEnabled.Should().BeTrue();
        campaign.FollowUpDueDays.Should().Be(5);
        campaign.FollowUpType.Should().Be(FollowUpTaskType.Call);
    }

    [Fact]
    public void ShouldRejectEnablingFollowUpWithNonPositiveDueDays()
    {
        var campaign = new Campaign("Autumn outreach", null, Guid.NewGuid(), [Guid.NewGuid()]);

        var act = () => campaign.ConfigureFollowUp(true, 0, FollowUpTaskType.Email);

        act.Should().Throw<DomainException>()
            .WithMessage("Follow-up due days must be greater than zero when follow-up is enabled.");
    }
}
