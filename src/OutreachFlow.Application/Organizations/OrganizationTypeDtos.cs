namespace OutreachFlow.Application.Organizations;

public sealed record OrganizationTypeDto(
    Guid Id,
    string Name,
    DateTimeOffset CreatedAt);

public sealed record CreateOrganizationTypeRequest(string Name);

public sealed record UpdateOrganizationTypeRequest(string Name);
