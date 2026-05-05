using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Application.Organizations;

public interface IOrganizationRepository
{
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);

    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Organization>> ListAsync(CancellationToken cancellationToken = default);

    void Remove(Organization organization);
}
