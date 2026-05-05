using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class SenderProfileRepository(OutreachFlowDbContext dbContext) : ISenderProfileRepository
{
    public async Task AddAsync(SenderProfile senderProfile, CancellationToken cancellationToken = default)
    {
        await dbContext.SenderProfiles.AddAsync(senderProfile, cancellationToken);
    }

    public async Task<SenderProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.SenderProfiles
            .FirstOrDefaultAsync(senderProfile => senderProfile.Id == id, cancellationToken);
    }

    public async Task<SenderProfile?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SenderProfiles
            .Where(senderProfile => senderProfile.IsActive && senderProfile.IsDefault)
            .OrderBy(senderProfile => senderProfile.Name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SenderProfile>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.SenderProfiles.AsQueryable();

        if (activeOnly is not null)
        {
            query = query.Where(senderProfile => senderProfile.IsActive == activeOnly);
        }

        return await query
            .OrderByDescending(senderProfile => senderProfile.IsDefault)
            .ThenBy(senderProfile => senderProfile.Name)
            .ToArrayAsync(cancellationToken);
    }

    public void Remove(SenderProfile senderProfile)
    {
        dbContext.SenderProfiles.Remove(senderProfile);
    }
}
