namespace OutreachFlow.Application.EmailSending;

public sealed record EmailSendResult(
    bool Success,
    string Provider,
    string? ProviderMessageId,
    string? ErrorMessage);
