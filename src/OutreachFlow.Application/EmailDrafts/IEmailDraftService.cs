namespace OutreachFlow.Application.EmailDrafts;

public interface IEmailDraftService
{
    Task<GenerateEmailDraftsResult> GenerateAsync(
        GenerateEmailDraftsRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmailDraftDto>> ListAsync(
        EmailDraftFilterRequest filter,
        CancellationToken cancellationToken = default);

    Task<EmailDraftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
