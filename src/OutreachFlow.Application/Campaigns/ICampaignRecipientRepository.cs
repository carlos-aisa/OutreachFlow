using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Application.Campaigns;

public interface ICampaignRecipientRepository
{
    Task AddAsync(CampaignRecipient recipient, CancellationToken cancellationToken = default);

    Task<CampaignRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CampaignRecipient?> GetByCampaignAndContactAsync(
        Guid campaignId,
        Guid contactId,
        Guid messageTemplateId,
        CancellationToken cancellationToken = default);

    Task<CampaignRecipient?> GetByEmailDraftIdAsync(Guid emailDraftId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignRecipient>> ListByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CampaignRecipient>> ListIncorporatedByCampaignAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> ListContactIdsByCampaignAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
