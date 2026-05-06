namespace OutreachFlow.Application.ContactImports;

public interface IContactImportService
{
    Task<ContactImportPreviewResult> PreviewAsync(
        ContactImportPreviewRequest request,
        CancellationToken cancellationToken = default);

    Task<ContactImportCommitResult> CommitAsync(
        ContactImportCommitRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ImportJobDto>> ListJobsAsync(
        int? limit,
        CancellationToken cancellationToken = default);
}
