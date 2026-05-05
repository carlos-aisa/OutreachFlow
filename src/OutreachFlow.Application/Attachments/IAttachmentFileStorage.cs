namespace OutreachFlow.Application.Attachments;

public interface IAttachmentFileStorage
{
    Task<StoredAttachmentFile> SaveAsync(
        AttachmentFileSaveRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record AttachmentFileSaveRequest(
    string FileName,
    string ContentType,
    Stream Content,
    long SizeBytes);

public sealed record StoredAttachmentFile(
    string FileName,
    string ContentType,
    string StoragePath,
    long SizeBytes);
