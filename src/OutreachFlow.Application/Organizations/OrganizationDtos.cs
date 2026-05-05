namespace OutreachFlow.Application.Organizations;

public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string? Type,
    string? Website,
    string? City,
    string? Province,
    string? Country,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateOrganizationRequest(
    string Name,
    string? Type,
    string? Website,
    string? City,
    string? Province,
    string? Country,
    string? Notes);

public sealed record UpdateOrganizationRequest(
    string Name,
    string? Type,
    string? Website,
    string? City,
    string? Province,
    string? Country,
    string? Notes);
