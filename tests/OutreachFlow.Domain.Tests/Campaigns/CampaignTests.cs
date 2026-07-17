using FluentAssertions;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Tests.Campaigns;

public sealed class CampaignTests
{
    [Fact]
    public void ShouldAllowPartiallyPreparedCampaign()
    {
        var campaign = new Campaign("Autumn outreach");
        campaign.UpdateDetails("Autumn outreach", subject: null, body: null, senderProfileId: null, followUpDueAt: null);
        campaign.Name.Should().Be("Autumn outreach");
        campaign.Subject.Should().BeNull();
    }

    [Fact]
    public void ShouldRejectCampaignWithoutName() =>
        FluentActions.Invoking(() => new Campaign(" ")).Should().Throw<DomainException>();
}
