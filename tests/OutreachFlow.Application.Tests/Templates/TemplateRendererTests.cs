using FluentAssertions;

using OutreachFlow.Application.Templates;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.Tests.Templates;

public sealed class TemplateRendererTests
{
    private readonly TemplateRenderer _renderer = new();

    [Fact]
    public void ShouldRenderSubjectAndBodyWhenAllVariablesHaveValues()
    {
        var template = new EmailTemplate(
            "Initial Outreach",
            "Template description",
            "Proposal for {{ organization.name }}",
            "Hello {{contact.displayName}}, your contact email is {{contact.email}}. Regards, {{sender.signature}}");

        var contact = new Contact(
            "Maria Lopez",
            "maria@example.com",
            role: "Partnership Manager");

        var organization = new Organization(
            "Acme Foundation",
            city: "Madrid",
            province: "Madrid");

        var senderProfile = new SenderProfile(
            "Carlos",
            "carlos@example.com",
            organizationName: "OutreachFlow",
            signature: "<p>Carlos A.</p>",
            signatureFormat: SenderSignatureFormat.Html);

        var context = new TemplateContext(contact, organization, senderProfile);

        var renderedEmail = _renderer.Render(template, context);

        renderedEmail.Subject.Should().Be("Proposal for Acme Foundation");
        renderedEmail.Body.Should().Be("Hello Maria Lopez, your contact email is maria@example.com. Regards, <p>Carlos A.</p>");
        renderedEmail.UnknownVariables.Should().BeEmpty();
        renderedEmail.MissingVariables.Should().BeEmpty();
        renderedEmail.HasErrors.Should().BeFalse();
    }

    [Fact]
    public void ShouldReportUnknownVariablesWhenTemplateContainsUnsupportedTokens()
    {
        var template = new EmailTemplate(
            "Unknown Variables",
            null,
            "Hello {{contact.unknownField}}",
            "Body");

        var context = CreateContext();

        var renderedEmail = _renderer.Render(template, context);

        renderedEmail.UnknownVariables.Should().Equal("contact.unknownField");
        renderedEmail.MissingVariables.Should().BeEmpty();
        renderedEmail.Subject.Should().Be("Hello {{contact.unknownField}}");
        renderedEmail.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void ShouldReportMissingVariablesWhenSupportedValuesAreNullOrWhitespace()
    {
        var template = new EmailTemplate(
            "Missing Variables",
            null,
            "Hello {{organization.name}}",
            "Regards, {{sender.signature}}");

        var contact = new Contact("Maria Lopez", "maria@example.com");
        var senderProfile = new SenderProfile(
            "Carlos",
            "carlos@example.com");

        var context = new TemplateContext(contact, null, senderProfile);

        var renderedEmail = _renderer.Render(template, context);

        renderedEmail.MissingVariables.Should().Equal("organization.name", "sender.signature");
        renderedEmail.UnknownVariables.Should().BeEmpty();
        renderedEmail.Subject.Should().Be("Hello {{organization.name}}");
        renderedEmail.Body.Should().Be("Regards, {{sender.signature}}");
        renderedEmail.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void ShouldKeepUnsupportedExpressionAsUnknownAndUnresolved()
    {
        var template = new EmailTemplate(
            "Expression Syntax",
            null,
            "Hello",
            "Hello {{contact.displayName | upper}}");

        var context = CreateContext();

        var renderedEmail = _renderer.Render(template, context);

        renderedEmail.UnknownVariables.Should().Equal("contact.displayName | upper");
        renderedEmail.MissingVariables.Should().BeEmpty();
        renderedEmail.Body.Should().Be("Hello {{contact.displayName | upper}}");
        renderedEmail.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void ShouldReportErrorsWhenTokenRemainsUnresolved()
    {
        var template = new EmailTemplate(
            "Unresolved Token",
            null,
            "Hello",
            "Body {{   }}");

        var context = CreateContext();

        var renderedEmail = _renderer.Render(template, context);

        renderedEmail.UnknownVariables.Should().BeEmpty();
        renderedEmail.MissingVariables.Should().BeEmpty();
        renderedEmail.Body.Should().Be("Body {{   }}");
        renderedEmail.HasErrors.Should().BeTrue();
    }

    private static TemplateContext CreateContext()
    {
        var contact = new Contact("Maria Lopez", "maria@example.com", role: "Partnership Manager");
        var organization = new Organization("Acme Foundation", city: "Madrid", province: "Madrid");
        var senderProfile = new SenderProfile(
            "Carlos",
            "carlos@example.com",
            signature: "<p>Carlos A.</p>",
            signatureFormat: SenderSignatureFormat.Html);

        return new TemplateContext(contact, organization, senderProfile);
    }
}
