using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.ContactImports;

public sealed class ImportJob
{
    private ImportJob()
    {
        FileName = string.Empty;
    }

    public ImportJob(
        string fileName,
        int totalRows,
        int validRows,
        int duplicateRows,
        int invalidRows,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        FileName = RequireFileName(fileName);
        TotalRows = RequireNonNegative(totalRows, nameof(totalRows));
        ValidRows = RequireNonNegative(validRows, nameof(validRows));
        DuplicateRows = RequireNonNegative(duplicateRows, nameof(duplicateRows));
        InvalidRows = RequireNonNegative(invalidRows, nameof(invalidRows));
        Status = ImportJobStatus.Pending;
        CreatedCount = 0;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string FileName { get; private set; }

    public ImportJobStatus Status { get; private set; }

    public int TotalRows { get; private set; }

    public int ValidRows { get; private set; }

    public int CreatedCount { get; private set; }

    public int DuplicateRows { get; private set; }

    public int InvalidRows { get; private set; }

    public string? FailureReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public void MarkCompleted(
        int createdCount,
        int duplicateRows,
        int invalidRows,
        DateTimeOffset completedAt)
    {
        CreatedCount = RequireNonNegative(createdCount, nameof(createdCount));
        DuplicateRows = RequireNonNegative(duplicateRows, nameof(duplicateRows));
        InvalidRows = RequireNonNegative(invalidRows, nameof(invalidRows));
        Status = ImportJobStatus.Completed;
        FailureReason = null;
        CompletedAt = completedAt;
    }

    public void MarkFailed(string reason, DateTimeOffset completedAt)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("Import failure reason is required.");
        }

        Status = ImportJobStatus.Failed;
        FailureReason = reason.Trim();
        CompletedAt = completedAt;
    }

    private static string RequireFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new DomainException("Import file name is required.");
        }

        return fileName.Trim();
    }

    private static int RequireNonNegative(int value, string paramName)
    {
        if (value < 0)
        {
            throw new DomainException($"{paramName} must be zero or positive.");
        }

        return value;
    }
}
