using OutreachFlow.Domain.Attachments;

namespace OutreachFlow.Application.Attachments;

public interface IAttachmentAssetRepository
{
    Task AddAsync(AttachmentAsset attachmentAsset, CancellationToken cancellationToken = default);

    Task<AttachmentAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AttachmentAsset>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default);
}
