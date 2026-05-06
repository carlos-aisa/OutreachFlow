using System.Net.Mail;

namespace OutreachFlow.Infrastructure.EmailSending;

public interface ISmtpTransport : IDisposable
{
    Task SendMailAsync(MailMessage message, CancellationToken cancellationToken);
}
