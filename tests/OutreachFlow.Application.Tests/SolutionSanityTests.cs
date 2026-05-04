using FluentAssertions;

namespace OutreachFlow.Application.Tests;

public sealed class SolutionSanityTests
{
    [Fact]
    public void ShouldRunApplicationTestProject()
    {
        true.Should().BeTrue();
    }
}
