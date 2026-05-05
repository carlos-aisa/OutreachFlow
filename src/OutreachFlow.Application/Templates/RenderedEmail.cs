namespace OutreachFlow.Application.Templates;

public sealed record RenderedEmail(
    string Subject,
    string Body,
    IReadOnlyList<string> MissingVariables,
    IReadOnlyList<string> UnknownVariables,
    bool HasErrors);
