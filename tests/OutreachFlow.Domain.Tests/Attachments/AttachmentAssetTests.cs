using FluentAssertions;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Tests.Attachments;

public sealed class AttachmentAssetTests
{
    [Fact]
    public void ShouldCreateActiveAttachmentAsset()
    {
        var asset = new AttachmentAsset(
            "Service brochure",
            "brochure.pdf",
            "application/pdf",
            "attachments/brochure.pdf",
            2048,
            "General introduction");

        asset.Id.Should().NotBeEmpty();
        asset.IsActive.Should().BeTrue();
        asset.Name.Should().Be("Service brochure");
        asset.SizeBytes.Should().Be(2048);
    }

    [Fact]
    public void ShouldRejectAttachmentWithInvalidSize()
    {
        var act = () => new AttachmentAsset(
            "Service brochure",
            "brochure.pdf",
            "application/pdf",
            "attachments/brochure.pdf",
            0);

        act.Should().Throw<DomainException>()
            .WithMessage("Attachment size must be greater than zero.");
    }

    [Fact]
    public void ShouldDeactivateAttachment()
    {
        var asset = new AttachmentAsset(
            "Service brochure",
            "brochure.pdf",
            "application/pdf",
            "attachments/brochure.pdf",
            2048);

        asset.Deactivate();

        asset.IsActive.Should().BeFalse();
    }
}
