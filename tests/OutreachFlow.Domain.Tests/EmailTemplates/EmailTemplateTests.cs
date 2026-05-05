using FluentAssertions;
using OutreachFlow.Domain.Attachments;
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

    [Fact]
    public void ShouldAssignAndRemoveDefaultAttachment()
    {
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var asset = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "attachments/brochure.pdf",
            2048);
        var updatedAt = DateTimeOffset.UtcNow;

        var assigned = template.AssignDefaultAttachment(asset, updatedAt);
        var removed = template.RemoveDefaultAttachment(asset.Id, updatedAt.AddMinutes(1));

        assigned.Should().BeTrue();
        removed.Should().BeTrue();
        template.DefaultAttachments.Should().BeEmpty();
    }

    [Fact]
    public void ShouldNotAssignDuplicateDefaultAttachment()
    {
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var asset = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "attachments/brochure.pdf",
            2048);
        var updatedAt = DateTimeOffset.UtcNow;
        template.AssignDefaultAttachment(asset, updatedAt);

        var assignedAgain = template.AssignDefaultAttachment(asset, updatedAt.AddMinutes(1));

        assignedAgain.Should().BeFalse();
        template.DefaultAttachments.Should().ContainSingle();
    }

    [Fact]
    public void ShouldRejectInactiveAttachmentAsDefault()
    {
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        var asset = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "attachments/brochure.pdf",
            2048);
        asset.Deactivate();
        var updatedAt = DateTimeOffset.UtcNow;

        var act = () => template.AssignDefaultAttachment(asset, updatedAt);

        act.Should().Throw<DomainException>()
            .WithMessage("Inactive attachments cannot be assigned to templates.");
    }
}
