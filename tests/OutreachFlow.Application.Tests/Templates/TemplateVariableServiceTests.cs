using FluentAssertions;
using OutreachFlow.Application.Templates;

namespace OutreachFlow.Application.Tests.Templates;

public sealed class TemplateVariableServiceTests
{
    [Fact]
    public void ShouldListSupportedTemplateVariables()
    {
        var service = new TemplateVariableService();
        var registryNames = TemplateVariableRegistry
            .ListSupported()
            .Select(variable => variable.Name)
            .ToArray();

        var variables = service.ListSupportedVariables();
        var variableNames = variables.Select(variable => variable.Name).ToArray();

        variableNames.Should().Equal(registryNames);
        variables.Should().OnlyContain(variable => !string.IsNullOrWhiteSpace(variable.Example));
    }
}
