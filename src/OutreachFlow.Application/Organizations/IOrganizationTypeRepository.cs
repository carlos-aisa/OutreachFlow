using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Application.Organizations;

public interface IOrganizationTypeRepository
{
    Task AddAsync(OrganizationType organizationType, CancellationToken cancellationToken = default);

    Task<OrganizationType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<OrganizationType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrganizationType>> ListAsync(CancellationToken cancellationToken = default);

    void Remove(OrganizationType organizationType);
}
