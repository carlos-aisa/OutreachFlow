namespace OutreachFlow.Application.Organizations;

public interface IOrganizationTypeService
{
    Task<OrganizationTypeDto> CreateAsync(CreateOrganizationTypeRequest request, CancellationToken cancellationToken = default);

    Task<OrganizationTypeDto> UpdateAsync(Guid id, UpdateOrganizationTypeRequest request, CancellationToken cancellationToken = default);

    Task<OrganizationTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OrganizationTypeDto>> ListAsync(CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
