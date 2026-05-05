using OutreachFlow.Application.EmailSending;

namespace OutreachFlow.Infrastructure.EmailSending;

public sealed class ConfiguredEmailSendingPolicy : IEmailSendingPolicy
{
    public ConfiguredEmailSendingPolicy(TimeSpan equivalentEmailWindow)
    {
        if (equivalentEmailWindow <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(equivalentEmailWindow),
                "Equivalent email window must be greater than zero.");
        }

        EquivalentEmailWindow = equivalentEmailWindow;
    }

    public TimeSpan EquivalentEmailWindow { get; }
}
