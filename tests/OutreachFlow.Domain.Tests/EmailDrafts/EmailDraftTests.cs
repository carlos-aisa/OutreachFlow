using FluentAssertions;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.EmailDrafts;

namespace OutreachFlow.Domain.Tests.EmailDrafts;

public sealed class EmailDraftTests
{
    [Fact]
    public void ShouldCreateDraftWhenRenderHasNoErrors()
    {
        var draft = EmailDraft.CreateGenerated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Subject",
            "Body",
            hasRenderErrors: false,
            missingVariablesJson: null,
            unknownVariablesJson: null);

        draft.Status.Should().Be(EmailDraftStatus.Draft);
        draft.HasRenderErrors.Should().BeFalse();
    }

    [Fact]
    public void ShouldCreateNeedsReviewDraftWhenRenderHasErrors()
    {
        var draft = EmailDraft.CreateGenerated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Subject",
            "Body",
            hasRenderErrors: true,
            missingVariablesJson: "[\"organization.name\"]",
            unknownVariablesJson: "[]");

        draft.Status.Should().Be(EmailDraftStatus.NeedsReview);
        draft.HasRenderErrors.Should().BeTrue();
        draft.MissingVariablesJson.Should().Be("[\"organization.name\"]");
    }

    [Fact]
    public void ShouldAssignActiveAttachmentToDraft()
    {
        var draft = CreateDraft();
        var attachment = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "storage/brochure.pdf",
            2048);

        var assigned = draft.AssignAttachment(attachment, DateTimeOffset.UtcNow);

        assigned.Should().BeTrue();
        draft.Attachments.Should().ContainSingle(item => item.AttachmentAssetId == attachment.Id);
    }

    [Fact]
    public void ShouldRejectInactiveAttachmentAssignment()
    {
        var draft = CreateDraft();
        var attachment = new AttachmentAsset(
            "Brochure",
            "brochure.pdf",
            "application/pdf",
            "storage/brochure.pdf",
            2048);
        attachment.Deactivate();

        var act = () => draft.AssignAttachment(attachment, DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Inactive attachments cannot be assigned to drafts.");
    }

    private static EmailDraft CreateDraft()
    {
        return EmailDraft.CreateGenerated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Subject",
            "Body",
            hasRenderErrors: false,
            missingVariablesJson: null,
            unknownVariablesJson: null);
    }
}
