namespace OutreachFlow.Domain.Campaigns;
public sealed class CampaignAttachment { private CampaignAttachment() { } public CampaignAttachment(Guid campaignId, Guid attachmentAssetId) { CampaignId = campaignId; AttachmentAssetId = attachmentAssetId; } public Guid CampaignId { get; private set; } public Guid AttachmentAssetId { get; private set; } }
