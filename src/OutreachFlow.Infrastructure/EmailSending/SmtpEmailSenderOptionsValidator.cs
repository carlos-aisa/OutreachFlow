namespace OutreachFlow.Infrastructure.EmailSending;

public static class SmtpEmailSenderOptionsValidator
{
    public static void ValidateConfigured(SmtpEmailSenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            missing.Add("EmailSending:Smtp:Host");
        }

        if (options.Port <= 0)
        {
            missing.Add("EmailSending:Smtp:Port");
        }

        if (string.IsNullOrWhiteSpace(options.Username))
        {
            missing.Add("EmailSending:Smtp:Username");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            missing.Add("EmailSending:Smtp:Password");
        }

        if (options.TimeoutSeconds <= 0)
        {
            missing.Add("EmailSending:Smtp:TimeoutSeconds");
        }

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"SMTP provider is not configured. Missing or invalid settings: {string.Join(", ", missing)}");
        }
    }
}
