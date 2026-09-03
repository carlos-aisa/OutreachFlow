using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Application.Organizations;

public sealed class OrganizationTypeService(
    IOrganizationTypeRepository organizationTypeRepository,
    IUnitOfWork unitOfWork)
    : IOrganizationTypeService
{
    public async Task<OrganizationTypeDto> CreateAsync(
        CreateOrganizationTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        var existing = await organizationTypeRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
        {
            throw new ApplicationConflictException("Organization type already exists.");
        }

        var organizationType = CreateOrganizationType(request);
        await organizationTypeRepository.AddAsync(organizationType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(organizationType);
    }

    public async Task<OrganizationTypeDto> UpdateAsync(
        Guid id,
        UpdateOrganizationTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        var organizationType = await organizationTypeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Organization type was not found.");

        var existing = await organizationTypeRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null && existing.Id != id)
        {
            throw new ApplicationConflictException("Organization type already exists.");
        }

        try
        {
            organizationType.Update(request.Name);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(organizationType);
    }

    public async Task<OrganizationTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organizationType = await organizationTypeRepository.GetByIdAsync(id, cancellationToken);
        return organizationType is null ? null : Map(organizationType);
    }

    public async Task<IReadOnlyList<OrganizationTypeDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var organizationTypes = await organizationTypeRepository.ListAsync(cancellationToken);
        return organizationTypes.Select(Map).ToArray();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organizationType = await organizationTypeRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Organization type was not found.");

        organizationTypeRepository.Remove(organizationType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static OrganizationType CreateOrganizationType(CreateOrganizationTypeRequest request)
    {
        try
        {
            return new OrganizationType(request.Name);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }

    private static OrganizationTypeDto Map(OrganizationType organizationType)
    {
        return new OrganizationTypeDto(organizationType.Id, organizationType.Name, organizationType.CreatedAt);
    }
}
