using Microsoft.Extensions.Options;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Infrastructure.FollowUps;

public sealed class ConfiguredFollowUpAutomationPolicy(
    IOptions<FollowUpAutomationOptions> options) : IFollowUpAutomationPolicy
{
    public bool AutoCreateAfterSuccessfulSend =>
        options.Value.AutoCreateAfterSuccessfulSend;

    public int AutoCreateDueDays => options.Value.DueDaysAfterSend <= 0
        ? 7
        : options.Value.DueDaysAfterSend;

    public FollowUpTaskType AutoCreateType => options.Value.DefaultType;
}
