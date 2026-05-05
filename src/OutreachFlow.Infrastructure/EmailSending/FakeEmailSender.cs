using Microsoft.Extensions.Options;
using OutreachFlow.Application.EmailSending;

namespace OutreachFlow.Infrastructure.EmailSending;

public sealed class FakeEmailSender(IOptions<EmailSendingOptions> options) : IEmailSender
{
    public Task<EmailSendResult> SendAsync(
        SendEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        var configuredOptions = options.Value;
        var failureKeyword = configuredOptions.FakeFailureKeyword;

        if (!string.IsNullOrWhiteSpace(failureKeyword))
        {
            var shouldFail =
                ContainsIgnoreCase(command.To, failureKeyword) ||
                ContainsIgnoreCase(command.Subject, failureKeyword) ||
                ContainsIgnoreCase(command.Body, failureKeyword);

            if (shouldFail)
            {
                return Task.FromResult(new EmailSendResult(
                    Success: false,
                    Provider: "Fake",
                    ProviderMessageId: null,
                    ErrorMessage: "Simulated failure from fake email sender."));
            }
        }

        return Task.FromResult(new EmailSendResult(
            Success: true,
            Provider: "Fake",
            ProviderMessageId: $"fake-{Guid.NewGuid():N}",
            ErrorMessage: null));
    }

    private static bool ContainsIgnoreCase(string value, string candidate)
    {
        return value.Contains(candidate, StringComparison.OrdinalIgnoreCase);
    }
}
