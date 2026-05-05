using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Application.Tags;

public interface ITagRepository
{
    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);

    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Tag?> GetByNameAsync(string name, string? category, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Tag>> ListAsync(CancellationToken cancellationToken = default);

    void Remove(Tag tag);
}
