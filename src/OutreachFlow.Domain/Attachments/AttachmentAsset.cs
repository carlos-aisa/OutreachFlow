using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Attachments;

public sealed class AttachmentAsset
{
    private AttachmentAsset()
    {
        Name = string.Empty;
        FileName = string.Empty;
        ContentType = string.Empty;
        StoragePath = string.Empty;
    }

    public AttachmentAsset(
        string name,
        string fileName,
        string contentType,
        string storagePath,
        long sizeBytes,
        string? description = null,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name, "Attachment name is required.");
        FileName = RequireText(fileName, "Attachment file name is required.");
        ContentType = RequireText(contentType, "Attachment content type is required.");
        StoragePath = RequireText(storagePath, "Attachment storage path is required.");
        SizeBytes = RequirePositiveSize(sizeBytes);
        Description = NormalizeOptional(description);
        IsActive = true;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public string StoragePath { get; private set; }

    public long SizeBytes { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public void UpdateMetadata(string name, string? description)
    {
        Name = RequireText(name, "Attachment name is required.");
        Description = NormalizeOptional(description);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static long RequirePositiveSize(long sizeBytes)
    {
        if (sizeBytes <= 0)
        {
            throw new DomainException("Attachment size must be greater than zero.");
        }

        return sizeBytes;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
