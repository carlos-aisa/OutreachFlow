namespace OutreachFlow.Application.FollowUps;

public interface IFollowUpTaskService
{
    Task<FollowUpTaskDto> CreateAsync(
        CreateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<FollowUpTaskDto> UpdateAsync(
        Guid id,
        UpdateFollowUpTaskRequest request,
        CancellationToken cancellationToken = default);

    Task<FollowUpTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FollowUpTaskDto>> ListAsync(
        FollowUpTaskFilterRequest filter,
        CancellationToken cancellationToken = default);

    Task<FollowUpTaskDto> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
}
