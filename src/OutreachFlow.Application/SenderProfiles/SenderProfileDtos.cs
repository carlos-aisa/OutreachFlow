using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.SenderProfiles;

public sealed record SenderProfileDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? OrganizationName,
    string? Website,
    string? Signature,
    SenderSignatureFormat? SignatureFormat,
    bool IsDefault,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateSenderProfileRequest(
    string Name,
    string Email,
    string? Phone,
    string? OrganizationName,
    string? Website,
    string? Signature,
    bool IsDefault,
    SenderSignatureFormat? SignatureFormat = null);

public sealed record UpdateSenderProfileRequest(
    string Name,
    string Email,
    string? Phone,
    string? OrganizationName,
    string? Website,
    string? Signature,
    bool IsDefault,
    bool IsActive,
    SenderSignatureFormat? SignatureFormat = null);
