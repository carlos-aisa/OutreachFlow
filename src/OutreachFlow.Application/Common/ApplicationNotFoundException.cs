namespace OutreachFlow.Application.Common;

public sealed class ApplicationNotFoundException : ApplicationLayerException
{
    public ApplicationNotFoundException(string message)
        : base("NOT_FOUND", message)
    {
    }
}
