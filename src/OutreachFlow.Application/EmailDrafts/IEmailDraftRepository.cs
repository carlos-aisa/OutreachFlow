using OutreachFlow.Domain.EmailDrafts;

namespace OutreachFlow.Application.EmailDrafts;

public interface IEmailDraftRepository
{
    Task AddRangeAsync(IReadOnlyList<EmailDraft> drafts, CancellationToken cancellationToken = default);

    Task<EmailDraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmailDraft>> ListAsync(
        EmailDraftFilterRequest filter,
        CancellationToken cancellationToken = default);
}
