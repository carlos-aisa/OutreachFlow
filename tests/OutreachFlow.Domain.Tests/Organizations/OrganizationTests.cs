using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Domain.Tests.Organizations;

public sealed class OrganizationTests
{
    [Fact]
    public void ConstructorShouldCreateOrganizationWhenNameIsValid()
    {
        var organization = new Organization("Example Organization", city: "Madrid");

        organization.Id.Should().NotBeEmpty();
        organization.Name.Should().Be("Example Organization");
        organization.City.Should().Be("Madrid");
        organization.CreatedAt.Should().Be(organization.UpdatedAt);
    }

    [Fact]
    public void ConstructorShouldRejectMissingName()
    {
        var act = () => new Organization(" ");

        act.Should().Throw<DomainException>()
            .WithMessage("Organization name is required.");
    }
}
