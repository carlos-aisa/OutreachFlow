namespace OutreachFlow.Application.Common;

public sealed class ApplicationConflictException : ApplicationLayerException
{
    public ApplicationConflictException(string message)
        : base("CONFLICT", message)
    {
    }
}
