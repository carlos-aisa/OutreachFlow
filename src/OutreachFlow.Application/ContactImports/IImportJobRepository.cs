using OutreachFlow.Domain.ContactImports;

namespace OutreachFlow.Application.ContactImports;

public interface IImportJobRepository
{
    Task AddAsync(ImportJob importJob, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ImportJob>> ListAsync(int? limit, CancellationToken cancellationToken = default);
}
