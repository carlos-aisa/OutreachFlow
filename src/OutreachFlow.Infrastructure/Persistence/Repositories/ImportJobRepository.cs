using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.ContactImports;
using OutreachFlow.Domain.ContactImports;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class ImportJobRepository(OutreachFlowDbContext dbContext)
    : IImportJobRepository
{
    public async Task AddAsync(ImportJob importJob, CancellationToken cancellationToken = default)
    {
        await dbContext.ImportJobs.AddAsync(importJob, cancellationToken);
    }

    public async Task<IReadOnlyList<ImportJob>> ListAsync(
        int? limit,
        CancellationToken cancellationToken = default)
    {
        var jobs = await dbContext.ImportJobs
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        IEnumerable<ImportJob> query = jobs.OrderByDescending(importJob => importJob.CreatedAt);

        if (limit is int limitValue && limitValue > 0)
        {
            query = query.Take(limitValue);
        }

        return query.ToArray();
    }
}
