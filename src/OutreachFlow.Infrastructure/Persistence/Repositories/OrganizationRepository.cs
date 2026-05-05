using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository(OutreachFlowDbContext dbContext) : IOrganizationRepository
{
    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await dbContext.Organizations.AddAsync(organization, cancellationToken);
    }

    public async Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Organizations
            .FirstOrDefaultAsync(organization => organization.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Organization>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Organizations
            .OrderBy(organization => organization.Name)
            .ToArrayAsync(cancellationToken);
    }

    public void Remove(Organization organization)
    {
        dbContext.Organizations.Remove(organization);
    }
}
