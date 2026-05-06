using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Domain.Tests.ContactActivities;

public sealed class ContactActivityTests
{
    [Fact]
    public void ShouldCreateActivityWithOptionalMetadata()
    {
        var occurredAt = DateTimeOffset.UtcNow;

        var activity = ContactActivity.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            ContactActivityType.EmailSent,
            "Subject line",
            "Preview content",
            "{\"provider\":\"fake\"}",
            occurredAt);

        activity.Type.Should().Be(ContactActivityType.EmailSent);
        activity.Subject.Should().Be("Subject line");
        activity.BodyPreview.Should().Be("Preview content");
        activity.MetadataJson.Should().Be("{\"provider\":\"fake\"}");
        activity.OccurredAt.Should().Be(occurredAt);
    }

    [Fact]
    public void ShouldRejectMissingContactId()
    {
        var act = () => ContactActivity.Create(
            Guid.Empty,
            organizationId: null,
            ContactActivityType.ContactCreated,
            subject: null,
            bodyPreview: null,
            metadataJson: null,
            DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Contact id is required.");
    }
}
