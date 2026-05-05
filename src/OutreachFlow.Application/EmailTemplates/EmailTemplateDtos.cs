namespace OutreachFlow.Application.EmailTemplates;

public sealed record EmailTemplateDto(
    Guid Id,
    string Name,
    string? Description,
    string SubjectTemplate,
    string BodyTemplate,
    IReadOnlyList<Guid> DefaultAttachmentIds,
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
