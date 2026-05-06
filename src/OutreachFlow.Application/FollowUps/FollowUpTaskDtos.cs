using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Application.FollowUps;

public sealed record FollowUpTaskDto(
    Guid Id,
    Guid ContactId,
    string ContactDisplayName,
    string ContactEmail,
    Guid? OrganizationId,
    DateTimeOffset DueAt,
    FollowUpTaskType Type,
    string? Notes,
    bool IsCompleted,
    DateTimeOffset? CompletedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record FollowUpTaskFilterRequest(
    Guid? ContactId,
    bool? IsCompleted,
    DateTimeOffset? DueFrom,
    DateTimeOffset? DueTo,
    int? Limit = null);

public sealed record CreateFollowUpTaskRequest(
    Guid ContactId,
    Guid? OrganizationId,
    DateTimeOffset DueAt,
    FollowUpTaskType Type,
    string? Notes);

public sealed record UpdateFollowUpTaskRequest(
    DateTimeOffset DueAt,
    FollowUpTaskType Type,
    string? Notes);
