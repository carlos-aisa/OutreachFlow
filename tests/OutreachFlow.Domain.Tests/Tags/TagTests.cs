using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Domain.Tests.Tags;

public sealed class TagTests
{
    [Fact]
    public void ConstructorShouldCreateTagWhenNameIsValid()
    {
        var tag = new Tag("Priority", "Lead");

        tag.Id.Should().NotBeEmpty();
        tag.Name.Should().Be("Priority");
        tag.NormalizedName.Should().Be("PRIORITY");
        tag.Category.Should().Be("Lead");
        tag.NormalizedCategory.Should().Be("LEAD");
    }

    [Fact]
    public void ConstructorShouldRejectMissingName()
    {
        var act = () => new Tag("");

        act.Should().Throw<DomainException>()
            .WithMessage("Tag name is required.");
    }
}
