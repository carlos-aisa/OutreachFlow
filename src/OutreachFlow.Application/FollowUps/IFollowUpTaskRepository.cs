using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Application.FollowUps;

public interface IFollowUpTaskRepository
{
    Task AddAsync(FollowUpTask task, CancellationToken cancellationToken = default);

    Task<FollowUpTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FollowUpTask>> ListAsync(
        FollowUpTaskFilterRequest filter,
        CancellationToken cancellationToken = default);
}
