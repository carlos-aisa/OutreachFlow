using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Application.FollowUps;

public interface IFollowUpAutomationPolicy
{
    bool AutoCreateAfterSuccessfulSend { get; }

    int AutoCreateDueDays { get; }

    FollowUpTaskType AutoCreateType { get; }
}
