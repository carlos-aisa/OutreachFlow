using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;

namespace OutreachFlow.Application.Attachments;

public sealed class AttachmentAssetService(
    IAttachmentAssetRepository attachmentAssetRepository,
    IAttachmentFileStorage attachmentFileStorage,
    IUnitOfWork unitOfWork)
    : IAttachmentAssetService
{
    public async Task<AttachmentAssetDto> UploadAsync(
        UploadAttachmentAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateUploadRequest(request);

        StoredAttachmentFile storedFile;

        try
        {
            storedFile = await attachmentFileStorage.SaveAsync(
                new AttachmentFileSaveRequest(
                    request.FileName,
                    request.ContentType,
                    request.Content,
                    request.SizeBytes),
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        AttachmentAsset attachmentAsset;

        try
        {
            attachmentAsset = new AttachmentAsset(
                request.Name,
                storedFile.FileName,
                storedFile.ContentType,
                storedFile.StoragePath,
                storedFile.SizeBytes,
                request.Description);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await attachmentAssetRepository.AddAsync(attachmentAsset, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(attachmentAsset);
    }

    public async Task<AttachmentAssetDto> UpdateAsync(
        Guid id,
        UpdateAttachmentAssetRequest request,
        CancellationToken cancellationToken = default)
    {
        var attachmentAsset = await attachmentAssetRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Attachment asset was not found.");

        try
        {
            attachmentAsset.UpdateMetadata(request.Name, request.Description);

            if (request.IsActive)
            {
                attachmentAsset.Activate();
            }
            else
            {
                attachmentAsset.Deactivate();
            }
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(attachmentAsset);
    }

    public async Task<AttachmentAssetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var attachmentAsset = await attachmentAssetRepository.GetByIdAsync(id, cancellationToken);
        return attachmentAsset is null ? null : Map(attachmentAsset);
    }

    public async Task<IReadOnlyList<AttachmentAssetDto>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var attachmentAssets = await attachmentAssetRepository.ListAsync(activeOnly, cancellationToken);
        return attachmentAssets.Select(Map).ToArray();
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var attachmentAsset = await attachmentAssetRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Attachment asset was not found.");

        attachmentAsset.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static AttachmentAssetDto Map(AttachmentAsset attachmentAsset)
    {
        return new AttachmentAssetDto(
            attachmentAsset.Id,
            attachmentAsset.Name,
            attachmentAsset.FileName,
            attachmentAsset.ContentType,
            attachmentAsset.StoragePath,
            attachmentAsset.SizeBytes,
            attachmentAsset.Description,
            attachmentAsset.IsActive,
            attachmentAsset.CreatedAt);
    }

    private static void ValidateUploadRequest(UploadAttachmentAssetRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ApplicationValidationException("Attachment name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new ApplicationValidationException("Attachment file name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            throw new ApplicationValidationException("Attachment content type is required.");
        }

        if (request.SizeBytes <= 0)
        {
            throw new ApplicationValidationException("Attachment file is required.");
        }
    }
}
