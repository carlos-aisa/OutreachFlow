using FluentAssertions;
using OutreachFlow.Application.Templates;

namespace OutreachFlow.Application.Tests.Templates;

public sealed class TemplateVariableServiceTests
{
    [Fact]
    public void ShouldListSupportedTemplateVariables()
    {
        var service = new TemplateVariableService();

        var variables = service.ListSupportedVariables();

        variables.Should().Contain(variable => variable.Name == "contact.displayName");
        variables.Should().Contain(variable => variable.Name == "sender.signature");
        variables.Should().OnlyContain(variable => !string.IsNullOrWhiteSpace(variable.Example));
    }
}
