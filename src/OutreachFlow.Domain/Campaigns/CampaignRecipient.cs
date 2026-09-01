using OutreachFlow.Domain.Common;
namespace OutreachFlow.Domain.Campaigns;
public sealed class CampaignRecipient
{
    private CampaignRecipient() { }
    public CampaignRecipient(Guid campaignId, Guid contactId, int messageVersion = 1)
    { if (campaignId == Guid.Empty || contactId == Guid.Empty) throw new DomainException("Campaign and contact ids are required."); CampaignId = campaignId; ContactId = contactId; MessageVersion = messageVersion; Status = CampaignRecipientStatus.Included; CreatedAt = DateTimeOffset.UtcNow; UpdatedAt = CreatedAt; }
    public Guid CampaignId { get; private set; } public Guid ContactId { get; private set; } public int MessageVersion { get; private set; } public CampaignRecipientStatus Status { get; private set; } public Guid? EmailDraftId { get; private set; } public DateTimeOffset CreatedAt { get; private set; } public DateTimeOffset UpdatedAt { get; private set; }
    public void MarkDraftGenerated(Guid draftId) { EmailDraftId = draftId; Status = CampaignRecipientStatus.DraftGenerated; UpdatedAt = DateTimeOffset.UtcNow; }
    public void MarkSent() { Status = CampaignRecipientStatus.Sent; UpdatedAt = DateTimeOffset.UtcNow; }
    public void Exclude() { Status = CampaignRecipientStatus.Excluded; UpdatedAt = DateTimeOffset.UtcNow; }
}
