namespace OutreachFlow.Application.EmailSending;

public interface IEmailSender
{
    Task<EmailSendResult> SendAsync(
        SendEmailCommand command,
        CancellationToken cancellationToken = default);
}
