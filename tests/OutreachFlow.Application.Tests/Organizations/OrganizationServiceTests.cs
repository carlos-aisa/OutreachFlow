using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tests.Support;

namespace OutreachFlow.Application.Tests.Organizations;

public sealed class OrganizationServiceTests
{
    [Fact]
    public async Task ShouldCreateOrganizationWhenRequestIsValid()
    {
        var organizationRepository = new InMemoryOrganizationRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new OrganizationService(organizationRepository, unitOfWork);

        var organization = await service.CreateAsync(new CreateOrganizationRequest(
            "Northwind Studio",
            "Customer",
            "https://example.com",
            "Madrid",
            "Madrid",
            "Spain",
            "Initial outreach target."));

        organization.Id.Should().NotBeEmpty();
        organization.Name.Should().Be("Northwind Studio");
        organizationRepository.Organizations.Should().ContainSingle();
        unitOfWork.SaveChangesCount.Should().Be(1);
    }

    [Fact]
    public async Task ShouldUpdateOrganizationWhenItExists()
    {
        var organizationRepository = new InMemoryOrganizationRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new OrganizationService(organizationRepository, unitOfWork);
        var organization = await service.CreateAsync(new CreateOrganizationRequest(
            "Northwind Studio",
            null,
            null,
            null,
            null,
            null,
            null));

        var updated = await service.UpdateAsync(organization.Id, new UpdateOrganizationRequest(
            "Northwind Consulting",
            "Partner",
            null,
            "Valencia",
            null,
            "Spain",
            "Updated notes."));

        updated.Name.Should().Be("Northwind Consulting");
        updated.Type.Should().Be("Partner");
        updated.City.Should().Be("Valencia");
        unitOfWork.SaveChangesCount.Should().Be(2);
    }

    [Fact]
    public async Task ShouldRejectOrganizationWithoutName()
    {
        var service = new OrganizationService(
            new InMemoryOrganizationRepository(),
            new InMemoryUnitOfWork());

        var act = () => service.CreateAsync(new CreateOrganizationRequest(
            " ",
            null,
            null,
            null,
            null,
            null,
            null));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Organization name is required.");
    }
}
