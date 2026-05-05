namespace OutreachFlow.Application.Common;

public sealed class ApplicationValidationException : ApplicationLayerException
{
    public ApplicationValidationException(string message)
        : base("VALIDATION_ERROR", message)
    {
    }
}
