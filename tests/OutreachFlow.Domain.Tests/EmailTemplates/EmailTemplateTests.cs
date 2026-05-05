using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Domain.Tests.EmailTemplates;

public sealed class EmailTemplateTests
{
    [Fact]
    public void ShouldCreateActiveEmailTemplate()
    {
        var template = new EmailTemplate(
            "Intro",
            "Initial outreach",
            "Hello {{contact.displayName}}",
            "Hello {{contact.displayName}},\n\n{{sender.signature}}");

        template.Id.Should().NotBeEmpty();
        template.Name.Should().Be("Intro");
        template.IsActive.Should().BeTrue();
        template.SubjectTemplate.Should().Be("Hello {{contact.displayName}}");
    }

    [Fact]
    public void ShouldRejectTemplateWithoutSubject()
    {
        var act = () => new EmailTemplate("Intro", null, " ", "Body");

        act.Should().Throw<DomainException>()
            .WithMessage("Email template subject is required.");
    }

    [Fact]
    public void ShouldDeactivateTemplate()
    {
        var template = new EmailTemplate("Intro", null, "Subject", "Body");

        template.Deactivate(DateTimeOffset.UtcNow);

        template.IsActive.Should().BeFalse();
    }
}
