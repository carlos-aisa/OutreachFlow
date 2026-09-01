using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Campaigns;

public sealed class CampaignAudienceGroup
{
    private CampaignAudienceGroup()
    {
    }

    public CampaignAudienceGroup(Guid campaignId, Guid contactGroupId)
    {
        if (campaignId == Guid.Empty)
        {
            throw new DomainException("Campaign id is required.");
        }

        if (contactGroupId == Guid.Empty)
        {
            throw new DomainException("Contact group id is required.");
        }

        CampaignId = campaignId;
        ContactGroupId = contactGroupId;
    }

    public Guid CampaignId { get; private set; }

    public Guid ContactGroupId { get; private set; }
}
