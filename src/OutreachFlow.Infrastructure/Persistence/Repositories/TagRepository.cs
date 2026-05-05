using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class TagRepository(OutreachFlowDbContext dbContext) : ITagRepository
{
    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        await dbContext.Tags.AddAsync(tag, cancellationToken);
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tags.FirstOrDefaultAsync(tag => tag.Id == id, cancellationToken);
    }

    public async Task<Tag?> GetByNameAsync(
        string name,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeKey(name);
        var normalizedCategory = NormalizeKey(category);

        return await dbContext.Tags.FirstOrDefaultAsync(
            tag => tag.NormalizedName == normalizedName &&
                tag.NormalizedCategory == normalizedCategory,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Tag>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tags
            .OrderBy(tag => tag.Category)
            .ThenBy(tag => tag.Name)
            .ToArrayAsync(cancellationToken);
    }

    public void Remove(Tag tag)
    {
        dbContext.Tags.Remove(tag);
    }

    private static string NormalizeKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
    }
}
