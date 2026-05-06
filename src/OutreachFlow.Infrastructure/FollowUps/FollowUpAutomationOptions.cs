using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Infrastructure.FollowUps;

public sealed class FollowUpAutomationOptions
{
    public const string SectionName = "FollowUpAutomation";

    public bool AutoCreateAfterSuccessfulSend { get; set; }

    public int DueDaysAfterSend { get; set; } = 7;

    public FollowUpTaskType DefaultType { get; set; } = FollowUpTaskType.Email;
}
