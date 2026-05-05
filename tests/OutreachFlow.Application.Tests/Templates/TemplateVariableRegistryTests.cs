using FluentAssertions;

using OutreachFlow.Application.Templates;

namespace OutreachFlow.Application.Tests.Templates;

public sealed class TemplateVariableRegistryTests
{
    [Fact]
    public void ShouldExposeExpectedSupportedVariableNames()
    {
        var names = TemplateVariableRegistry
            .ListSupported()
            .Select(variable => variable.Name)
            .ToArray();

        names.Should().Equal(
            "contact.displayName",
            "contact.email",
            "contact.role",
            "organization.name",
            "organization.city",
            "organization.province",
            "sender.name",
            "sender.email",
            "sender.phone",
            "sender.organizationName",
            "sender.website",
            "sender.signature");
    }
}
