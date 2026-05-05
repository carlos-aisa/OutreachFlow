namespace OutreachFlow.Application.Common;

public abstract class ApplicationLayerException : Exception
{
    protected ApplicationLayerException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; }
}
