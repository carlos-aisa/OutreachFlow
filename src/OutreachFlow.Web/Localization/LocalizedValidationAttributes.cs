using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Resources;

namespace OutreachFlow.Web;

internal static class SharedValidationMessages
{
    private static readonly ResourceManager ResourceManager = new(
        "OutreachFlow.Web.Resources.SharedResource",
        typeof(SharedResource).Assembly);

    public static string Get(string key, string fallback)
    {
        return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? fallback;
    }
}

public sealed class LocalizedRequiredAttribute : RequiredAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return SharedValidationMessages.Get("Validation.Required", "This field is required.");
    }
}

public sealed class LocalizedEmailAddressAttribute : ValidationAttribute
{
    private static readonly EmailAddressAttribute InnerAttribute = new();

    public override bool IsValid(object? value)
    {
        return InnerAttribute.IsValid(value);
    }

    public override string FormatErrorMessage(string name)
    {
        return SharedValidationMessages.Get("Validation.EmailAddress", "Enter a valid email address.");
    }
}
