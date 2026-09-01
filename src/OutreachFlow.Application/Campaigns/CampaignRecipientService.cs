using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.Common;

namespace OutreachFlow.Application.Campaigns;

public sealed class CampaignRecipientService(
    ICampaignRepository campaignRepository,
    ICampaignRecipientRepository campaignRecipientRepository,
    IContactRepository contactRepository,
    IContactGroupService contactGroupService,
    IEmailDraftService emailDraftService,
    IUnitOfWork unitOfWork)
    : ICampaignRecipientService
{
    public async Task<IReadOnlyList<CampaignRecipientCandidateDto>> DiscoverCandidatesAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);

        var memberContactIds = new HashSet<Guid>();

        foreach (var audienceGroup in campaign.AudienceGroups)
        {
            var members = await contactGroupService.ListMembersAsync(audienceGroup.ContactGroupId, cancellationToken);

            foreach (var member in members)
            {
                memberContactIds.Add(member.ContactId);
            }
        }

        var existingContactIds = (await campaignRecipientRepository.ListContactIdsByCampaignAsync(campaignId, cancellationToken))
            .ToHashSet();
        var candidateContactIds = memberContactIds.Except(existingContactIds).ToArray();

        var candidates = new List<CampaignRecipientCandidateDto>(candidateContactIds.Length);

        foreach (var contactId in candidateContactIds)
        {
            var contact = await contactRepository.GetByIdAsync(contactId, cancellationToken);

            if (contact is null)
            {
                continue;
            }

            candidates.Add(new CampaignRecipientCandidateDto(contact.Id, contact.DisplayName, contact.Email));
        }

        return candidates.OrderBy(candidate => candidate.DisplayName).ToArray();
    }

    public async Task<CampaignRecipientDto> IncorporateAsync(
        Guid campaignId,
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);

        if (campaign.Status != CampaignStatus.Open)
        {
            throw new ApplicationValidationException("Only open campaigns can incorporate recipients.");
        }

        _ = await contactRepository.GetByIdAsync(contactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        var existing = await campaignRecipientRepository.GetByCampaignAndContactAsync(
            campaignId,
            contactId,
            campaign.EmailTemplateId,
            cancellationToken);

        if (existing is not null)
        {
            throw new ApplicationConflictException(
                "This contact is already incorporated into the campaign for its current message.");
        }

        CampaignRecipient recipient;

        try
        {
            recipient = new CampaignRecipient(campaignId, contactId, campaign.EmailTemplateId);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await campaignRecipientRepository.AddAsync(recipient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapAsync(recipient, cancellationToken);
    }

    public async Task<IReadOnlyList<CampaignRecipientDto>> ListAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        _ = await FindCampaignAsync(campaignId, cancellationToken);

        var recipients = await campaignRecipientRepository.ListByCampaignAsync(campaignId, cancellationToken);
        var result = new List<CampaignRecipientDto>(recipients.Count);

        foreach (var recipient in recipients)
        {
            result.Add(await MapAsync(recipient, cancellationToken));
        }

        return result;
    }

    public async Task<GenerateCampaignDraftsResult> GenerateDraftsAsync(
        Guid campaignId,
        GenerateCampaignDraftsRequest request,
        CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);

        if (campaign.Status != CampaignStatus.Open)
        {
            throw new ApplicationValidationException("Only open campaigns can generate drafts.");
        }

        var incorporated = await campaignRecipientRepository.ListIncorporatedByCampaignAsync(campaignId, cancellationToken);

        if (incorporated.Count == 0)
        {
            return new GenerateCampaignDraftsResult(0, 0, 0, []);
        }

        var generation = await emailDraftService.GenerateForContactsAsync(
            new GenerateEmailDraftsForContactsRequest(
                incorporated.Select(recipient => recipient.ContactId).ToArray(),
                campaign.EmailTemplateId,
                request.SenderProfileId,
                request.AttachmentAssetIds),
            cancellationToken);

        var recipientByContactId = incorporated.ToDictionary(recipient => recipient.ContactId);

        foreach (var draft in generation.Drafts)
        {
            if (recipientByContactId.TryGetValue(draft.ContactId, out var recipient))
            {
                recipient.AssignDraft(draft.Id);
            }
        }

        foreach (var skipped in generation.Skipped)
        {
            if (recipientByContactId.TryGetValue(skipped.ContactId, out var recipient))
            {
                recipient.Exclude(skipped.Reason);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var recipientDtos = new List<CampaignRecipientDto>(incorporated.Count);

        foreach (var recipient in incorporated)
        {
            recipientDtos.Add(await MapAsync(recipient, cancellationToken));
        }

        return new GenerateCampaignDraftsResult(
            incorporated.Count,
            generation.Drafts.Count,
            generation.Skipped.Count,
            recipientDtos);
    }

    private async Task<Campaign> FindCampaignAsync(Guid campaignId, CancellationToken cancellationToken) =>
        await campaignRepository.GetByIdAsync(campaignId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Campaign was not found.");

    private async Task<CampaignRecipientDto> MapAsync(CampaignRecipient recipient, CancellationToken cancellationToken)
    {
        var contact = await contactRepository.GetByIdAsync(recipient.ContactId, cancellationToken);

        return new CampaignRecipientDto(
            recipient.Id,
            recipient.CampaignId,
            recipient.ContactId,
            contact?.DisplayName ?? string.Empty,
            contact?.Email ?? string.Empty,
            recipient.MessageTemplateId,
            recipient.Status,
            recipient.EmailDraftId,
            recipient.ExclusionReason,
            recipient.IncorporatedAt,
            recipient.UpdatedAt);
    }
}
