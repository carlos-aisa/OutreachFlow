namespace OutreachFlow.Application.EmailTemplates;

public sealed record EmailTemplateDto(
    Guid Id,
    string Name,
    string? Description,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateEmailTemplateRequest(
    string Name,
    string? Description,
    string SubjectTemplate,
    string BodyTemplate);

public sealed record UpdateEmailTemplateRequest(
    string Name,
    string? Description,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive);
