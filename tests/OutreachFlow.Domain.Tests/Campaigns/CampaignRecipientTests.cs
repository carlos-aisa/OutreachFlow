using FluentAssertions;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Tests.Campaigns;

public sealed class CampaignRecipientTests
{
    [Fact]
    public void ShouldIncorporateAsIncorporated()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        recipient.Id.Should().NotBeEmpty();
        recipient.Status.Should().Be(CampaignRecipientStatus.Incorporated);
        recipient.EmailDraftId.Should().BeNull();
    }

    [Fact]
    public void ShouldRejectEmptyCampaignId()
    {
        var act = () => new CampaignRecipient(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Campaign id is required.");
    }

    [Fact]
    public void ShouldRejectEmptyContactId()
    {
        var act = () => new CampaignRecipient(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Contact id is required.");
    }

    [Fact]
    public void ShouldRejectEmptyMessageTemplateId()
    {
        var act = () => new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        act.Should().Throw<DomainException>()
            .WithMessage("Message template id is required.");
    }

    [Fact]
    public void ShouldAssignDraftAndTransitionToDrafted()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var draftId = Guid.NewGuid();

        recipient.AssignDraft(draftId);

        recipient.Status.Should().Be(CampaignRecipientStatus.Drafted);
        recipient.EmailDraftId.Should().Be(draftId);
    }

    [Fact]
    public void ShouldRejectAssigningDraftTwice()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        recipient.AssignDraft(Guid.NewGuid());

        var act = () => recipient.AssignDraft(Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Only incorporated recipients can be assigned a draft.");
    }

    [Fact]
    public void ShouldMarkSentAfterDrafted()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        recipient.AssignDraft(Guid.NewGuid());

        recipient.MarkSent();

        recipient.Status.Should().Be(CampaignRecipientStatus.Sent);
    }

    [Fact]
    public void ShouldRejectMarkingSentBeforeDrafted()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var act = () => recipient.MarkSent();

        act.Should().Throw<DomainException>()
            .WithMessage("Only drafted recipients can be marked as sent.");
    }

    [Fact]
    public void ShouldMarkFailedAfterDrafted()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        recipient.AssignDraft(Guid.NewGuid());

        recipient.MarkFailed();

        recipient.Status.Should().Be(CampaignRecipientStatus.Failed);
    }

    [Fact]
    public void ShouldExcludeIncorporatedRecipient()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        recipient.Exclude("Contact is marked as Do Not Contact.");

        recipient.Status.Should().Be(CampaignRecipientStatus.Excluded);
        recipient.ExclusionReason.Should().Be("Contact is marked as Do Not Contact.");
    }

    [Fact]
    public void ShouldExcludeDraftedRecipient()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        recipient.AssignDraft(Guid.NewGuid());

        recipient.Exclude("Contact became do-not-contact.");

        recipient.Status.Should().Be(CampaignRecipientStatus.Excluded);
    }

    [Fact]
    public void ShouldRejectExcludingSentRecipient()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        recipient.AssignDraft(Guid.NewGuid());
        recipient.MarkSent();

        var act = () => recipient.Exclude("Too late.");

        act.Should().Throw<DomainException>()
            .WithMessage("Sent, failed, or already excluded recipients cannot be excluded.");
    }

    [Fact]
    public void ShouldRejectExcludingWithoutReason()
    {
        var recipient = new CampaignRecipient(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var act = () => recipient.Exclude(" ");

        act.Should().Throw<DomainException>()
            .WithMessage("An exclusion reason is required.");
    }
}
