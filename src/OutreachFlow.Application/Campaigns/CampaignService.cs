using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Application.Campaigns;

public sealed class CampaignService(
    ICampaignRepository campaignRepository,
    IEmailTemplateRepository emailTemplateRepository,
    IContactGroupRepository contactGroupRepository,
    IUnitOfWork unitOfWork)
    : ICampaignService
{
    public async Task<CampaignDto> CreateAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureTemplateExistsAsync(request.EmailTemplateId, cancellationToken);
        await EnsureGroupsExistAsync(request.AudienceGroupIds, cancellationToken);

        Campaign campaign;

        try
        {
            campaign = new Campaign(
                request.Name,
                request.Description,
                request.EmailTemplateId,
                request.AudienceGroupIds,
                request.FollowUpEnabled,
                request.FollowUpDueDays,
                request.FollowUpType);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await campaignRepository.AddAsync(campaign, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(campaign);
    }

    public async Task<CampaignDto> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default) =>
        Map(await FindCampaignAsync(campaignId, cancellationToken));

    public async Task<IReadOnlyList<CampaignDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var campaigns = await campaignRepository.ListAsync(cancellationToken);
        return campaigns.Select(Map).ToArray();
    }

    public async Task<CampaignDto> UpdateAsync(Guid campaignId, UpdateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);
        await EnsureTemplateExistsAsync(request.EmailTemplateId, cancellationToken);

        try
        {
            campaign.Rename(request.Name, request.Description);
            campaign.ChangeMessage(request.EmailTemplateId);
            campaign.ConfigureFollowUp(request.FollowUpEnabled, request.FollowUpDueDays, request.FollowUpType);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(campaign);
    }

    public async Task<CampaignDto> AddAudienceGroupAsync(Guid campaignId, Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);
        await EnsureGroupExistsAsync(contactGroupId, cancellationToken);

        try
        {
            campaign.AddAudienceGroup(contactGroupId);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(campaign);
    }

    public async Task<CampaignDto> RemoveAudienceGroupAsync(Guid campaignId, Guid contactGroupId, CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);

        try
        {
            campaign.RemoveAudienceGroup(contactGroupId);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(campaign);
    }

    public async Task<CampaignDto> CloseAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);

        try
        {
            campaign.Close();
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(campaign);
    }

    public async Task<CampaignDto> ReopenAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        var campaign = await FindCampaignAsync(campaignId, cancellationToken);

        try
        {
            campaign.Reopen();
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(campaign);
    }

    private async Task<Campaign> FindCampaignAsync(Guid campaignId, CancellationToken cancellationToken) =>
        await campaignRepository.GetByIdAsync(campaignId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Campaign was not found.");

    private async Task EnsureTemplateExistsAsync(Guid emailTemplateId, CancellationToken cancellationToken)
    {
        if (await emailTemplateRepository.GetByIdAsync(emailTemplateId, cancellationToken) is null)
        {
            throw new ApplicationNotFoundException("Email template was not found.");
        }
    }

    private async Task EnsureGroupExistsAsync(Guid contactGroupId, CancellationToken cancellationToken)
    {
        if (await contactGroupRepository.GetByIdAsync(contactGroupId, cancellationToken) is null)
        {
            throw new ApplicationNotFoundException("Contact group was not found.");
        }
    }

    private async Task EnsureGroupsExistAsync(IReadOnlyList<Guid> contactGroupIds, CancellationToken cancellationToken)
    {
        foreach (var contactGroupId in contactGroupIds)
        {
            await EnsureGroupExistsAsync(contactGroupId, cancellationToken);
        }
    }

    private static CampaignDto Map(Campaign campaign) =>
        new(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.EmailTemplateId,
            campaign.Status,
            campaign.AudienceGroups
                .Select(audienceGroup => audienceGroup.ContactGroupId)
                .OrderBy(id => id)
                .ToArray(),
            campaign.FollowUpEnabled,
            campaign.FollowUpDueDays,
            campaign.FollowUpType,
            campaign.CreatedAt,
            campaign.UpdatedAt);
}
