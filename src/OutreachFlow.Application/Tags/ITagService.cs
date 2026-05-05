namespace OutreachFlow.Application.Tags;

public interface ITagService
{
    Task<TagDto> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default);

    Task<TagDto> UpdateAsync(Guid id, UpdateTagRequest request, CancellationToken cancellationToken = default);

    Task<TagDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TagDto>> ListAsync(CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
