namespace OutreachFlow.Infrastructure.EmailSending;

public sealed class SmtpEmailSenderOptions
{
    public const string SectionName = "EmailSending:Smtp";

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public bool UseSsl { get; set; } = true;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 30;
}
