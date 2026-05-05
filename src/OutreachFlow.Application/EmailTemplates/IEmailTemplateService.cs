namespace OutreachFlow.Application.EmailTemplates;

public interface IEmailTemplateService
{
    Task<EmailTemplateDto> CreateAsync(CreateEmailTemplateRequest request, CancellationToken cancellationToken = default);

    Task<EmailTemplateDto> UpdateAsync(Guid id, UpdateEmailTemplateRequest request, CancellationToken cancellationToken = default);

    Task<EmailTemplateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmailTemplateDto>> ListAsync(bool? activeOnly, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
