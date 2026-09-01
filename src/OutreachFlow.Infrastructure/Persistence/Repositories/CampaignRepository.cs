using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class CampaignRepository(OutreachFlowDbContext dbContext) : ICampaignRepository
{
    public async Task AddAsync(Campaign campaign, CancellationToken cancellationToken = default) =>
        await dbContext.Campaigns.AddAsync(campaign, cancellationToken);

    public Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Campaigns
            .Include(campaign => campaign.AudienceGroups)
            .FirstOrDefaultAsync(campaign => campaign.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Campaign>> ListAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Campaigns
            .Include(campaign => campaign.AudienceGroups)
            .OrderBy(campaign => campaign.Name)
            .ToArrayAsync(cancellationToken);
}
