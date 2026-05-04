using FluentAssertions;

namespace OutreachFlow.Domain.Tests;

public sealed class SolutionSanityTests
{
    [Fact]
    public void ShouldRunDomainTestProject()
    {
        true.Should().BeTrue();
    }
}
