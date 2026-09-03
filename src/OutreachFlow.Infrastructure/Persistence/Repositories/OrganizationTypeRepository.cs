using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class OrganizationTypeRepository(OutreachFlowDbContext dbContext) : IOrganizationTypeRepository
{
    public async Task AddAsync(OrganizationType organizationType, CancellationToken cancellationToken = default)
    {
        await dbContext.OrganizationTypes.AddAsync(organizationType, cancellationToken);
    }

    public async Task<OrganizationType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.OrganizationTypes.FirstOrDefaultAsync(
            organizationType => organizationType.Id == id, cancellationToken);
    }

    public async Task<OrganizationType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeKey(name);

        return await dbContext.OrganizationTypes.FirstOrDefaultAsync(
            organizationType => organizationType.NormalizedName == normalizedName,
            cancellationToken);
    }

    public async Task<IReadOnlyList<OrganizationType>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.OrganizationTypes
            .OrderBy(organizationType => organizationType.Name)
            .ToArrayAsync(cancellationToken);
    }

    public void Remove(OrganizationType organizationType)
    {
        dbContext.OrganizationTypes.Remove(organizationType);
    }

    private static string NormalizeKey(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
    }
}
