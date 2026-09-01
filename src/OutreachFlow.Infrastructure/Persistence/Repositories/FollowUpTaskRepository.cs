using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class FollowUpTaskRepository(OutreachFlowDbContext dbContext) : IFollowUpTaskRepository
{
    public async Task AddAsync(FollowUpTask task, CancellationToken cancellationToken = default)
    {
        await dbContext.FollowUpTasks.AddAsync(task, cancellationToken);
    }

    public async Task<FollowUpTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.FollowUpTasks
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FollowUpTask>> ListAsync(
        FollowUpTaskFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.FollowUpTasks.AsQueryable();

        if (filter.ContactId is not null)
        {
            query = query.Where(task => task.ContactId == filter.ContactId);
        }

        if (filter.IsCompleted is bool isCompleted)
        {
            query = query.Where(task => task.IsCompleted == isCompleted);
        }

        var tasks = await query.ToArrayAsync(cancellationToken);
        IEnumerable<FollowUpTask> filteredTasks = tasks;

        if (filter.DueFrom is DateTimeOffset dueFrom)
        {
            filteredTasks = filteredTasks.Where(task => task.DueAt >= dueFrom);
        }

        if (filter.DueTo is DateTimeOffset dueTo)
        {
            filteredTasks = filteredTasks.Where(task => task.DueAt <= dueTo);
        }

        IEnumerable<FollowUpTask> orderedTasks = filteredTasks
            .OrderBy(task => task.DueAt)
            .ThenBy(task => task.CreatedAt);

        if (filter.Limit is int limit && limit > 0)
        {
            orderedTasks = orderedTasks.Take(limit);
        }

        return orderedTasks.ToArray();
    }
}
