namespace OutreachFlow.Application.Tags;

public sealed record TagDto(
    Guid Id,
    string Name,
    string? Category,
    DateTimeOffset CreatedAt);

public sealed record CreateTagRequest(string Name, string? Category);

public sealed record UpdateTagRequest(string Name, string? Category);
