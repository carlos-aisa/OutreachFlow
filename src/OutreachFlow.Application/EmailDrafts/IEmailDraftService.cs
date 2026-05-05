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

    Task<EmailDraftDto> UpdateAsync(
        Guid id,
        UpdateEmailDraftRequest request,
        CancellationToken cancellationToken = default);

    Task<EmailDraftDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmailDraftDto> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<EmailDraftDto> SendApprovedDraftAsync(Guid id, CancellationToken cancellationToken = default);
}
