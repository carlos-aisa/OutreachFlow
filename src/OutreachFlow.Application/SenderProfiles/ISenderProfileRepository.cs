using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.SenderProfiles;

public interface ISenderProfileRepository
{
    Task AddAsync(SenderProfile senderProfile, CancellationToken cancellationToken = default);

    Task<SenderProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SenderProfile?> GetDefaultAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SenderProfile>> ListAsync(bool? activeOnly, CancellationToken cancellationToken = default);

    void Remove(SenderProfile senderProfile);
}
