using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OutreachFlow.Application.Attachments;
using System.Globalization;

namespace OutreachFlow.Infrastructure.Storage;

public sealed class LocalAttachmentFileStorage : IAttachmentFileStorage
{
    private readonly string _storageRoot;

    public LocalAttachmentFileStorage(
        IOptions<AttachmentStorageOptions> options,
        IHostEnvironment hostEnvironment)
    {
        var configuredRootPath = options.Value.RootPath;

        if (string.IsNullOrWhiteSpace(configuredRootPath))
        {
            throw new InvalidOperationException("Attachment storage root path is required.");
        }

        _storageRoot = Path.IsPathRooted(configuredRootPath)
            ? Path.GetFullPath(configuredRootPath)
            : Path.GetFullPath(Path.Combine(hostEnvironment.ContentRootPath, configuredRootPath));
    }

    public async Task<StoredAttachmentFile> SaveAsync(
        AttachmentFileSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        var safeFileName = EnsureSafeFileName(request.FileName);
        var extension = Path.GetExtension(safeFileName);
        var utcNow = DateTimeOffset.UtcNow;
        var relativeStoragePath = Path.Combine(
                utcNow.Year.ToString("0000", CultureInfo.InvariantCulture),
                utcNow.Month.ToString("00", CultureInfo.InvariantCulture),
                $"{Guid.NewGuid():N}{extension}")
            .Replace('\\', '/');

        var fullStoragePath = Path.GetFullPath(Path.Combine(_storageRoot, relativeStoragePath));
        EnsurePathInsideRoot(fullStoragePath);

        var targetDirectory = Path.GetDirectoryName(fullStoragePath)
            ?? throw new InvalidOperationException("Invalid attachment storage path.");
        Directory.CreateDirectory(targetDirectory);

        await using var targetStream = new FileStream(
            fullStoragePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);

        await request.Content.CopyToAsync(targetStream, cancellationToken);

        return new StoredAttachmentFile(
            safeFileName,
            request.ContentType.Trim(),
            relativeStoragePath,
            request.SizeBytes);
    }

    private static void ValidateRequest(AttachmentFileSaveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            throw new InvalidOperationException("Attachment file name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            throw new InvalidOperationException("Attachment content type is required.");
        }

        if (request.SizeBytes <= 0)
        {
            throw new InvalidOperationException("Attachment file is required.");
        }
    }

    private static string EnsureSafeFileName(string fileName)
    {
        var trimmedFileName = fileName.Trim();
        var normalizedFileName = trimmedFileName.Replace('\\', '/');

        if (normalizedFileName.Contains("../", StringComparison.Ordinal) ||
            normalizedFileName.Contains("/..", StringComparison.Ordinal) ||
            normalizedFileName.Contains('/', StringComparison.Ordinal) ||
            !string.Equals(Path.GetFileName(trimmedFileName), trimmedFileName, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Attachment file name contains an unsafe path.");
        }

        return trimmedFileName;
    }

    private void EnsurePathInsideRoot(string fullStoragePath)
    {
        var relativePath = Path.GetRelativePath(_storageRoot, fullStoragePath);

        if (relativePath.StartsWith("..", StringComparison.Ordinal) ||
            Path.IsPathRooted(relativePath))
        {
            throw new InvalidOperationException("Attachment file path resolved outside the configured storage root.");
        }
    }
}
