using System.Net;
using System.Net.Mail;

namespace OutreachFlow.Infrastructure.EmailSending;

public sealed class SystemSmtpTransport(SmtpClient smtpClient) : ISmtpTransport
{
    public async Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
#pragma warning disable CA2016 // SmtpClient.SendMailAsync does not accept CancellationToken.
        await smtpClient.SendMailAsync(message).WaitAsync(cancellationToken);
#pragma warning restore CA2016
    }

    public void Dispose()
    {
        smtpClient.Dispose();
    }
}

public sealed class SystemSmtpTransportFactory : ISmtpTransportFactory
{
    public ISmtpTransport Create(SmtpEmailSenderOptions options)
    {
        var smtpClient = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = options.UseSsl,
            Credentials = new NetworkCredential(options.Username, options.Password),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Timeout = options.TimeoutSeconds * 1000
        };

        return new SystemSmtpTransport(smtpClient);
    }
}
