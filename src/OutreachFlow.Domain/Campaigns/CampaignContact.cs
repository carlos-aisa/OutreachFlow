namespace OutreachFlow.Domain.Campaigns;
public sealed class CampaignContact { private CampaignContact() { } public CampaignContact(Guid campaignId, Guid contactId) { CampaignId = campaignId; ContactId = contactId; } public Guid CampaignId { get; private set; } public Guid ContactId { get; private set; } }
