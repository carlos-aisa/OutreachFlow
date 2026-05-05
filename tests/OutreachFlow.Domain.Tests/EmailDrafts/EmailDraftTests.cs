using FluentAssertions;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.EmailDrafts;
using System.Reflection;

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
    public void ShouldUpdateDraftAndClearDiagnosticsWhenContentIsResolved()
    {
        var createdAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var updatedAt = DateTimeOffset.UtcNow;
        var draft = EmailDraft.CreateGenerated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Subject {{organization.name}}",
            "Body with unresolved token.",
            hasRenderErrors: true,
            missingVariablesJson: "[\"organization.name\"]",
            unknownVariablesJson: "[]",
            createdAt);

        draft.UpdateContent("Resolved subject", "Resolved body", updatedAt);

        draft.Status.Should().Be(EmailDraftStatus.Draft);
        draft.HasRenderErrors.Should().BeFalse();
        draft.MissingVariablesJson.Should().BeNull();
        draft.UnknownVariablesJson.Should().BeNull();
        draft.UpdatedAt.Should().Be(updatedAt);
        draft.ApprovedAt.Should().BeNull();
    }

    [Fact]
    public void ShouldSetNeedsReviewWhenUpdatedContentContainsUnresolvedTokens()
    {
        var draft = CreateDraft();

        draft.UpdateContent(
            "Subject {{contact.displayName}}",
            "Resolved body",
            DateTimeOffset.UtcNow);

        draft.Status.Should().Be(EmailDraftStatus.NeedsReview);
        draft.HasRenderErrors.Should().BeTrue();
    }

    [Fact]
    public void ShouldApproveDraftWithoutErrors()
    {
        var approvedAt = DateTimeOffset.UtcNow;
        var draft = CreateDraft();

        draft.Approve(approvedAt);

        draft.Status.Should().Be(EmailDraftStatus.Approved);
        draft.ApprovedAt.Should().Be(approvedAt);
        draft.UpdatedAt.Should().Be(approvedAt);
    }

    [Fact]
    public void ShouldMarkApprovedDraftAsSent()
    {
        var draft = CreateDraft();
        var approvedAt = DateTimeOffset.UtcNow.AddMinutes(-2);
        var sentAt = DateTimeOffset.UtcNow;
        draft.Approve(approvedAt);

        draft.MarkSent(sentAt);

        draft.Status.Should().Be(EmailDraftStatus.Sent);
        draft.SentAt.Should().Be(sentAt);
        draft.FailureReason.Should().BeNull();
    }

    [Fact]
    public void ShouldMarkApprovedDraftAsFailed()
    {
        var draft = CreateDraft();
        var approvedAt = DateTimeOffset.UtcNow.AddMinutes(-2);
        var failedAt = DateTimeOffset.UtcNow;
        draft.Approve(approvedAt);

        draft.MarkFailed("Provider timeout.", failedAt);

        draft.Status.Should().Be(EmailDraftStatus.Failed);
        draft.FailureReason.Should().Be("Provider timeout.");
        draft.SentAt.Should().BeNull();
    }

    [Fact]
    public void ShouldRejectMarkingNonApprovedDraftAsSent()
    {
        var draft = CreateDraft();

        var act = () => draft.MarkSent(DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Only approved drafts can be marked as sent.");
    }

    [Fact]
    public void ShouldRejectApprovalWhenUnresolvedVariableTokenRemains()
    {
        var draft = EmailDraft.CreateGenerated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Subject {{contact.displayName}}",
            "Body",
            hasRenderErrors: false,
            missingVariablesJson: null,
            unknownVariablesJson: null);

        var act = () => draft.Approve(DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Draft cannot be approved while unresolved template variables remain.");
    }

    [Fact]
    public void ShouldRejectApprovalWhenRenderErrorsRemain()
    {
        var draft = EmailDraft.CreateGenerated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Subject",
            "Body",
            hasRenderErrors: true,
            missingVariablesJson: null,
            unknownVariablesJson: null);

        var act = () => draft.Approve(DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Draft cannot be approved while render errors remain.");
    }

    [Fact]
    public void ShouldCancelUnsentDraft()
    {
        var cancelledAt = DateTimeOffset.UtcNow;
        var draft = CreateDraft();

        draft.Cancel(cancelledAt);

        draft.Status.Should().Be(EmailDraftStatus.Cancelled);
        draft.CancelledAt.Should().Be(cancelledAt);
        draft.UpdatedAt.Should().Be(cancelledAt);
    }

    [Fact]
    public void ShouldRejectCancellingSentDraft()
    {
        var draft = CreateDraft();
        SetDraftStatusForTesting(draft, EmailDraftStatus.Sent);

        var act = () => draft.Cancel(DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Sent drafts cannot be cancelled.");
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

    private static void SetDraftStatusForTesting(EmailDraft draft, EmailDraftStatus status)
    {
        var property = typeof(EmailDraft).GetProperty(
            nameof(EmailDraft.Status),
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.Should().NotBeNull();
        property!.SetValue(draft, status);
    }
}
