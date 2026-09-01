using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Campaigns;

public sealed class CampaignRecipient
{
    private CampaignRecipient()
    {
    }

    public CampaignRecipient(
        Guid campaignId,
        Guid contactId,
        Guid messageTemplateId,
        DateTimeOffset? incorporatedAt = null)
    {
        if (campaignId == Guid.Empty)
        {
            throw new DomainException("Campaign id is required.");
        }

        if (contactId == Guid.Empty)
        {
            throw new DomainException("Contact id is required.");
        }

        if (messageTemplateId == Guid.Empty)
        {
            throw new DomainException("Message template id is required.");
        }

        Id = Guid.NewGuid();
        CampaignId = campaignId;
        ContactId = contactId;
        MessageTemplateId = messageTemplateId;
        Status = CampaignRecipientStatus.Incorporated;
        IncorporatedAt = incorporatedAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = IncorporatedAt;
    }

    public Guid Id { get; private set; }

    public Guid CampaignId { get; private set; }

    public Guid ContactId { get; private set; }

    public Guid MessageTemplateId { get; private set; }

    public CampaignRecipientStatus Status { get; private set; }

    public Guid? EmailDraftId { get; private set; }

    public string? ExclusionReason { get; private set; }

    public DateTimeOffset IncorporatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void AssignDraft(Guid emailDraftId, DateTimeOffset? updatedAt = null)
    {
        if (Status != CampaignRecipientStatus.Incorporated)
        {
            throw new DomainException("Only incorporated recipients can be assigned a draft.");
        }

        if (emailDraftId == Guid.Empty)
        {
            throw new DomainException("Email draft id is required.");
        }

        EmailDraftId = emailDraftId;
        Status = CampaignRecipientStatus.Drafted;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public void MarkSent(DateTimeOffset? updatedAt = null)
    {
        if (Status != CampaignRecipientStatus.Drafted)
        {
            throw new DomainException("Only drafted recipients can be marked as sent.");
        }

        Status = CampaignRecipientStatus.Sent;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public void MarkFailed(DateTimeOffset? updatedAt = null)
    {
        if (Status != CampaignRecipientStatus.Drafted)
        {
            throw new DomainException("Only drafted recipients can be marked as failed.");
        }

        Status = CampaignRecipientStatus.Failed;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public void Exclude(string reason, DateTimeOffset? updatedAt = null)
    {
        if (Status is CampaignRecipientStatus.Sent or CampaignRecipientStatus.Failed or CampaignRecipientStatus.Excluded)
        {
            throw new DomainException("Sent, failed, or already excluded recipients cannot be excluded.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("An exclusion reason is required.");
        }

        Status = CampaignRecipientStatus.Excluded;
        ExclusionReason = reason.Trim();
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
}
