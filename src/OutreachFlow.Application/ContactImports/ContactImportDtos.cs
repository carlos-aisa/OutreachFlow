using OutreachFlow.Domain.ContactImports;

namespace OutreachFlow.Application.ContactImports;

public sealed record ContactImportPreviewRequest(
    string FileName,
    string CsvContent);

public sealed record ContactImportCommitRequest(
    string FileName,
    string CsvContent,
    IReadOnlyList<Guid>? TagIds);

public sealed record ContactImportPreviewRowDto(
    int RowNumber,
    string? DisplayName,
    string? Email,
    string? Phone,
    string? Role,
    string? Source,
    bool IsValid,
    bool IsDuplicate,
    string? Message);

public sealed record ContactImportPreviewResult(
    int TotalRows,
    int ValidRows,
    int DuplicateRows,
    int InvalidRows,
    IReadOnlyList<ContactImportPreviewRowDto> Rows);

public sealed record ContactImportCommitResult(
    Guid ImportJobId,
    int TotalRows,
    int CreatedCount,
    int DuplicateCount,
    int InvalidCount);

public sealed record ImportJobDto(
    Guid Id,
    string FileName,
    ImportJobStatus Status,
    int TotalRows,
    int ValidRows,
    int CreatedCount,
    int DuplicateRows,
    int InvalidRows,
    string? FailureReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);
