using System.Net.Mail;

using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Contacts;

public static class EmailAddress
{
    public static string Normalize(string email)
    {
        var trimmedEmail = RequireValid(email);
        return trimmedEmail.ToUpperInvariant();
    }

    public static string RequireValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

        var trimmedEmail = email.Trim();

        try
        {
            var mailAddress = new MailAddress(trimmedEmail);

            if (!string.Equals(mailAddress.Address, trimmedEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException("Email format is invalid.");
            }
        }
        catch (FormatException exception)
        {
            throw new DomainException("Email format is invalid.", exception);
        }

        return trimmedEmail;
    }
}
