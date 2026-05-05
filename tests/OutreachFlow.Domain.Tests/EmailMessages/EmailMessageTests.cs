using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.EmailMessages;

namespace OutreachFlow.Domain.Tests.EmailMessages;

public sealed class EmailMessageTests
{
    [Fact]
    public void ShouldCreateSentEmailMessage()
    {
        var sentAt = DateTimeOffset.UtcNow;

        var message = EmailMessage.CreateSent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "alex@example.com",
            "Subject",
            "Body",
            "Fake",
            "provider-id",
            sentAt);

        message.Status.Should().Be(EmailMessageStatus.Sent);
        message.SentAt.Should().Be(sentAt);
        message.ProviderMessageId.Should().Be("provider-id");
    }

    [Fact]
    public void ShouldCreateFailedEmailMessage()
    {
        var occurredAt = DateTimeOffset.UtcNow;

        var message = EmailMessage.CreateFailed(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "alex@example.com",
            "Subject",
            "Body",
            "Fake",
            "Failure reason.",
            occurredAt);

        message.Status.Should().Be(EmailMessageStatus.Failed);
        message.FailureReason.Should().Be("Failure reason.");
        message.SentAt.Should().BeNull();
    }

    [Fact]
    public void ShouldRejectMessageWithoutRecipient()
    {
        var act = () => EmailMessage.CreateSent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            " ",
            "Subject",
            "Body",
            "Fake",
            null,
            DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Message recipient is required.");
    }
}
