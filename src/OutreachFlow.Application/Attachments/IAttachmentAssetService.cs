namespace OutreachFlow.Application.Attachments;

public interface IAttachmentAssetService
{
    Task<AttachmentAssetDto> UploadAsync(
        UploadAttachmentAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<AttachmentAssetDto> UpdateAsync(
        Guid id,
        UpdateAttachmentAssetRequest request,
        CancellationToken cancellationToken = default);

    Task<AttachmentAssetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AttachmentAssetDto>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
