using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Application.Organizations;

public sealed class OrganizationService(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork)
    : IOrganizationService
{
    public async Task<OrganizationDto> CreateAsync(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var organization = CreateOrganization(request);

        await organizationRepository.AddAsync(organization, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(organization);
    }

    public async Task<OrganizationDto> UpdateAsync(
        Guid id,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Organization was not found.");

        organization.Update(
            request.Name,
            request.Type,
            request.Website,
            request.City,
            request.Province,
            request.Country,
            request.Notes,
            DateTimeOffset.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(organization);
    }

    public async Task<OrganizationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.GetByIdAsync(id, cancellationToken);
        return organization is null ? null : Map(organization);
    }

    public async Task<IReadOnlyList<OrganizationDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var organizations = await organizationRepository.ListAsync(cancellationToken);
        return organizations.Select(Map).ToArray();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Organization was not found.");

        organizationRepository.Remove(organization);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static Organization CreateOrganization(CreateOrganizationRequest request)
    {
        try
        {
            return new Organization(
                request.Name,
                request.Type,
                request.Website,
                request.City,
                request.Province,
                request.Country,
                request.Notes);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }

    private static OrganizationDto Map(Organization organization)
    {
        return new OrganizationDto(
            organization.Id,
            organization.Name,
            organization.Type,
            organization.Website,
            organization.City,
            organization.Province,
            organization.Country,
            organization.Notes,
            organization.CreatedAt,
            organization.UpdatedAt);
    }
}
