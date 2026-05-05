namespace OutreachFlow.Application.Attachments;

public sealed record AttachmentAssetDto(
    Guid Id,
    string Name,
    string FileName,
    string ContentType,
    string StoragePath,
    long SizeBytes,
    string? Description,
    bool IsActive,
    DateTimeOffset CreatedAt);

public sealed record UploadAttachmentAssetRequest(
    string Name,
    string? Description,
    string FileName,
    string ContentType,
    Stream Content,
    long SizeBytes);

public sealed record UpdateAttachmentAssetRequest(
    string Name,
    string? Description,
    bool IsActive);
