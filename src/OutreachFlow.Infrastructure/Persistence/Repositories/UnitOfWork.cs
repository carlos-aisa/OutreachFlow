using OutreachFlow.Application.Common;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(OutreachFlowDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
