using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Domain.EmailDrafts;

namespace OutreachFlow.Application.Campaigns;

public interface ICampaignDraftGenerationService
{
    Task<int> GenerateAsync(Guid campaignId, CancellationToken cancellationToken = default);
}

public sealed class CampaignDraftGenerationService(
    ICampaignService campaignService,
    IContactGroupService contactGroupService,
    IContactRepository contactRepository,
    ISenderProfileRepository senderProfileRepository,
    IAttachmentAssetRepository attachmentAssetRepository,
    IEmailDraftRepository emailDraftRepository,
    IUnitOfWork unitOfWork) : ICampaignDraftGenerationService
{
    public async Task<int> GenerateAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await campaignService.GetAsync(campaignId, cancellationToken);
        if (campaign.MissingPrerequisites.Count > 0)
        {
            throw new ApplicationValidationException($"Campaign is not ready: {string.Join(", ", campaign.MissingPrerequisites)}.");
        }

        var sender = await senderProfileRepository.GetByIdAsync(campaign.SenderProfileId!.Value, cancellationToken)
            ?? throw new ApplicationNotFoundException("Campaign sender profile was not found.");
        if (!sender.IsActive) throw new ApplicationValidationException("Inactive sender profiles cannot be used for campaign draft generation.");

        var recipientIds = campaign.ContactIds.ToHashSet();
        foreach (var groupId in campaign.ContactGroupIds)
        {
            foreach (var member in await contactGroupService.ListMembersAsync(groupId, cancellationToken)) recipientIds.Add(member.ContactId);
        }

        var attachments = new List<OutreachFlow.Domain.Attachments.AttachmentAsset>();
        foreach (var attachmentId in campaign.AttachmentAssetIds)
        {
            var attachment = await attachmentAssetRepository.GetByIdAsync(attachmentId, cancellationToken) ?? throw new ApplicationNotFoundException("Campaign attachment was not found.");
            if (!attachment.IsActive) throw new ApplicationValidationException("Inactive attachments cannot be used for campaign draft generation.");
            attachments.Add(attachment);
        }

        var drafts = new List<EmailDraft>();
        foreach (var recipientId in recipientIds)
        {
            var contact = await contactRepository.GetByIdAsync(recipientId, cancellationToken) ?? throw new ApplicationNotFoundException("Campaign contact was not found.");
            if (contact.DoNotContact || string.IsNullOrWhiteSpace(contact.Email)) continue;
            var draft = EmailDraft.CreateCampaignGenerated(contact.Id, contact.OrganizationId, sender.Id, campaign.Subject!, campaign.Body!, DateTimeOffset.UtcNow);
            foreach (var attachment in attachments) draft.AssignAttachment(attachment, DateTimeOffset.UtcNow);
            drafts.Add(draft);
        }
        if (drafts.Count > 0) { await emailDraftRepository.AddRangeAsync(drafts, cancellationToken); await unitOfWork.SaveChangesAsync(cancellationToken); }
        return drafts.Count;
    }
}
