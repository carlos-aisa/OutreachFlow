using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.ContactActivities;

public sealed record ContactActivityDto(
    Guid Id,
    Guid ContactId,
    Guid? OrganizationId,
    ContactActivityType Type,
    string? Subject,
    string? BodyPreview,
    string? MetadataJson,
    DateTimeOffset OccurredAt);

public sealed record CreateContactActivityRequest(
    Guid ContactId,
    Guid? OrganizationId,
    ContactActivityType Type,
    string? Subject,
    string? BodyPreview,
    string? MetadataJson,
    DateTimeOffset? OccurredAt = null);
