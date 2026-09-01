using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Application.Campaigns;

public interface ICampaignRepository
{
    Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default);

    Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken cancellationToken = default);
}
