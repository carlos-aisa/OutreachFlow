namespace OutreachFlow.Infrastructure.EmailSending;

public interface ISmtpTransportFactory
{
    ISmtpTransport Create(SmtpEmailSenderOptions options);
}
