using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OutreachFlow.Application.EmailSending;

namespace OutreachFlow.Infrastructure.EmailSending;

public sealed class SmtpEmailSender(
    IOptions<SmtpEmailSenderOptions> smtpOptions,
    ISmtpTransportFactory smtpTransportFactory,
    ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task<EmailSendResult> SendAsync(
        SendEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var options = smtpOptions.Value;

        try
        {
            using var message = CreateMailMessage(command);
            using var transport = smtpTransportFactory.Create(options);
            await transport.SendMailAsync(message, cancellationToken);

            return new EmailSendResult(
                Success: true,
                Provider: "SMTP",
                ProviderMessageId: null,
                ErrorMessage: null);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "SMTP send failed for recipient {Recipient}.",
                command.To);

            return new EmailSendResult(
                Success: false,
                Provider: "SMTP",
                ProviderMessageId: null,
                ErrorMessage: BuildFailureMessage(exception));
        }
    }

    private static MailMessage CreateMailMessage(SendEmailCommand command)
    {
        var message = new MailMessage
        {
            From = new MailAddress(command.Sender.Email, command.Sender.Name),
            Subject = command.Subject,
            Body = command.Body,
            IsBodyHtml = false
        };
        message.To.Add(command.To);

        foreach (var attachment in command.Attachments)
        {
            if (!File.Exists(attachment.StoragePath))
            {
                throw new FileNotFoundException(
                    $"Attachment file was not found at path '{attachment.StoragePath}'.",
                    attachment.StoragePath);
            }

            var mailAttachment = new Attachment(attachment.StoragePath, attachment.ContentType)
            {
                Name = attachment.FileName
            };
            message.Attachments.Add(mailAttachment);
        }

        return message;
    }

    private static string BuildFailureMessage(Exception exception)
    {
        var baseMessage = string.IsNullOrWhiteSpace(exception.Message)
            ? "SMTP send failed."
            : $"SMTP send failed: {exception.Message}";

        return baseMessage.Length > 500 ? baseMessage[..500] : baseMessage;
    }
}
