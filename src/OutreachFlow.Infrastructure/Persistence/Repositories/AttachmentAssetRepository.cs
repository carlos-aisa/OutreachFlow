using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Domain.Attachments;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class AttachmentAssetRepository(OutreachFlowDbContext dbContext) : IAttachmentAssetRepository
{
    public async Task AddAsync(AttachmentAsset attachmentAsset, CancellationToken cancellationToken = default)
    {
        await dbContext.AttachmentAssets.AddAsync(attachmentAsset, cancellationToken);
    }

    public async Task<AttachmentAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.AttachmentAssets
            .FirstOrDefaultAsync(attachmentAsset => attachmentAsset.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<AttachmentAsset>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.AttachmentAssets.AsQueryable();

        if (activeOnly is not null)
        {
            query = query.Where(attachmentAsset => attachmentAsset.IsActive == activeOnly);
        }

        return await query
            .OrderBy(attachmentAsset => attachmentAsset.Name)
            .ThenBy(attachmentAsset => attachmentAsset.FileName)
            .ToArrayAsync(cancellationToken);
    }
}
