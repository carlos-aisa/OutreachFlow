using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Domain.Tests.FollowUps;

public sealed class FollowUpTaskTests
{
    [Fact]
    public void ShouldCreateFollowUpTaskWithDefaults()
    {
        var dueAt = DateTimeOffset.UtcNow.AddDays(3);

        var task = new FollowUpTask(
            Guid.NewGuid(),
            null,
            dueAt,
            FollowUpTaskType.Email,
            "Call after proposal.");

        task.IsCompleted.Should().BeFalse();
        task.CompletedAt.Should().BeNull();
        task.DueAt.Should().Be(dueAt);
    }

    [Fact]
    public void ShouldCompleteFollowUpTaskOnce()
    {
        var task = new FollowUpTask(
            Guid.NewGuid(),
            null,
            DateTimeOffset.UtcNow.AddDays(2),
            FollowUpTaskType.Call);

        task.Complete(DateTimeOffset.UtcNow);

        task.IsCompleted.Should().BeTrue();
        task.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void ShouldRejectCompletingTaskTwice()
    {
        var task = new FollowUpTask(
            Guid.NewGuid(),
            null,
            DateTimeOffset.UtcNow.AddDays(2),
            FollowUpTaskType.Call);
        task.Complete(DateTimeOffset.UtcNow);

        var act = () => task.Complete(DateTimeOffset.UtcNow.AddMinutes(1));

        act.Should().Throw<DomainException>()
            .WithMessage("Follow-up task is already completed.");
    }
}
