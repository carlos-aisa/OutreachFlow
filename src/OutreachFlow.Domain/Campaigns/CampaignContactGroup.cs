namespace OutreachFlow.Domain.Campaigns;
public sealed class CampaignContactGroup { private CampaignContactGroup() { } public CampaignContactGroup(Guid campaignId, Guid contactGroupId) { CampaignId = campaignId; ContactGroupId = contactGroupId; } public Guid CampaignId { get; private set; } public Guid ContactGroupId { get; private set; } }
