namespace OutreachFlow.Application.EmailSending;

public interface IEmailSendingPolicy
{
    TimeSpan EquivalentEmailWindow { get; }
}
