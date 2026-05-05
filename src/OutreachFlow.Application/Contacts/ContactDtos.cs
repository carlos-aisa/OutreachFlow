using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Application.Contacts;

public sealed record ContactTagDto(Guid Id, string Name, string? Category);

public sealed record ContactDto(
    Guid Id,
    Guid? OrganizationId,
    string? OrganizationName,
    string DisplayName,
    string Email,
    string? Phone,
    string? Role,
    string? Source,
    ContactStatus Status,
    bool DoNotContact,
    DateTimeOffset? LastContactedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<ContactTagDto> Tags);

public sealed record CreateContactRequest(
    Guid? OrganizationId,
    string DisplayName,
    string Email,
    string? Phone,
    string? Role,
    string? Source,
    ContactStatus Status,
    bool DoNotContact);

public sealed record UpdateContactRequest(
    Guid? OrganizationId,
    string DisplayName,
    string Email,
    string? Phone,
    string? Role,
    string? Source,
    ContactStatus Status,
    bool DoNotContact);

public sealed record ContactFilterRequest(
    string? Search,
    Guid? TagId,
    ContactStatus? Status,
    bool? DoNotContact,
    Guid? OrganizationId,
    DateTimeOffset? LastContactedFrom,
    DateTimeOffset? LastContactedTo);
