namespace OutreachFlow.Application.SenderProfiles;

public interface ISenderProfileService
{
    Task<SenderProfileDto> CreateAsync(CreateSenderProfileRequest request, CancellationToken cancellationToken = default);

    Task<SenderProfileDto> UpdateAsync(Guid id, UpdateSenderProfileRequest request, CancellationToken cancellationToken = default);

    Task<SenderProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SenderProfileDto?> GetDefaultAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SenderProfileDto>> ListAsync(bool? activeOnly, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
