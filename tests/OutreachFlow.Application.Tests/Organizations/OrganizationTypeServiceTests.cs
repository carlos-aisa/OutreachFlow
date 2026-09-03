using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tests.Support;

namespace OutreachFlow.Application.Tests.Organizations;

public sealed class OrganizationTypeServiceTests
{
    [Fact]
    public async Task ShouldCreateOrganizationTypeWhenRequestIsValid()
    {
        var repository = new InMemoryOrganizationTypeRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new OrganizationTypeService(repository, unitOfWork);

        var organizationType = await service.CreateAsync(new CreateOrganizationTypeRequest("Colegio"));

        organizationType.Id.Should().NotBeEmpty();
        organizationType.Name.Should().Be("Colegio");
        repository.OrganizationTypes.Should().ContainSingle();
        unitOfWork.SaveChangesCount.Should().Be(1);
    }

    [Fact]
    public async Task ShouldRejectDuplicateOrganizationType()
    {
        var repository = new InMemoryOrganizationTypeRepository();
        var service = new OrganizationTypeService(repository, new InMemoryUnitOfWork());
        await service.CreateAsync(new CreateOrganizationTypeRequest("Colegio"));

        var act = () => service.CreateAsync(new CreateOrganizationTypeRequest(" colegio "));

        await act.Should().ThrowAsync<ApplicationConflictException>()
            .WithMessage("Organization type already exists.");
    }

    [Fact]
    public async Task ShouldUpdateOrganizationTypeWhenItExists()
    {
        var repository = new InMemoryOrganizationTypeRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new OrganizationTypeService(repository, unitOfWork);
        var organizationType = await service.CreateAsync(new CreateOrganizationTypeRequest("Colegio"));

        var updated = await service.UpdateAsync(organizationType.Id, new UpdateOrganizationTypeRequest("Universidad"));

        updated.Name.Should().Be("Universidad");
        unitOfWork.SaveChangesCount.Should().Be(2);
    }

    [Fact]
    public async Task ShouldDeleteOrganizationTypeWhenItExists()
    {
        var repository = new InMemoryOrganizationTypeRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new OrganizationTypeService(repository, unitOfWork);
        var organizationType = await service.CreateAsync(new CreateOrganizationTypeRequest("Colegio"));

        await service.DeleteAsync(organizationType.Id);

        repository.OrganizationTypes.Should().BeEmpty();
        unitOfWork.SaveChangesCount.Should().Be(2);
    }

    [Fact]
    public async Task ShouldThrowWhenDeletingUnknownOrganizationType()
    {
        var repository = new InMemoryOrganizationTypeRepository();
        var service = new OrganizationTypeService(repository, new InMemoryUnitOfWork());

        var act = () => service.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<ApplicationNotFoundException>();
    }
}
