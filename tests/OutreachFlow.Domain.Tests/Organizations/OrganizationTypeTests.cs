using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Organizations;

namespace OutreachFlow.Domain.Tests.Organizations;

public sealed class OrganizationTypeTests
{
    [Fact]
    public void ConstructorShouldCreateOrganizationTypeWhenNameIsValid()
    {
        var organizationType = new OrganizationType("Colegio");

        organizationType.Id.Should().NotBeEmpty();
        organizationType.Name.Should().Be("Colegio");
        organizationType.NormalizedName.Should().Be("COLEGIO");
    }

    [Fact]
    public void ConstructorShouldRejectMissingName()
    {
        var act = () => new OrganizationType("");

        act.Should().Throw<DomainException>()
            .WithMessage("Organization type name is required.");
    }

    [Fact]
    public void UpdateShouldChangeNameAndNormalizedName()
    {
        var organizationType = new OrganizationType("Colegio");

        organizationType.Update("Universidad");

        organizationType.Name.Should().Be("Universidad");
        organizationType.NormalizedName.Should().Be("UNIVERSIDAD");
    }
}
