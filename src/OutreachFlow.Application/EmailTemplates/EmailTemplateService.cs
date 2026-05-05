using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Application.EmailTemplates;

public sealed class EmailTemplateService(
    IEmailTemplateRepository emailTemplateRepository,
    IUnitOfWork unitOfWork)
    : IEmailTemplateService
{
    public async Task<EmailTemplateDto> CreateAsync(
        CreateEmailTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailTemplate = CreateEmailTemplate(request);
        await emailTemplateRepository.AddAsync(emailTemplate, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(emailTemplate);
    }

    public async Task<EmailTemplateDto> UpdateAsync(
        Guid id,
        UpdateEmailTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailTemplate = await emailTemplateRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email template was not found.");

        try
        {
            emailTemplate.Update(
                request.Name,
                request.Description,
                request.SubjectTemplate,
                request.BodyTemplate,
                request.IsActive,
                DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(emailTemplate);
    }

    public async Task<EmailTemplateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var emailTemplate = await emailTemplateRepository.GetByIdAsync(id, cancellationToken);
        return emailTemplate is null ? null : Map(emailTemplate);
    }

    public async Task<IReadOnlyList<EmailTemplateDto>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var emailTemplates = await emailTemplateRepository.ListAsync(activeOnly, cancellationToken);
        return emailTemplates.Select(Map).ToArray();
    }

    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var emailTemplate = await emailTemplateRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email template was not found.");

        emailTemplate.Deactivate(DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static EmailTemplate CreateEmailTemplate(CreateEmailTemplateRequest request)
    {
        try
        {
            return new EmailTemplate(
                request.Name,
                request.Description,
                request.SubjectTemplate,
                request.BodyTemplate);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }

    private static EmailTemplateDto Map(EmailTemplate emailTemplate)
    {
        return new EmailTemplateDto(
            emailTemplate.Id,
            emailTemplate.Name,
            emailTemplate.Description,
            emailTemplate.SubjectTemplate,
            emailTemplate.BodyTemplate,
            emailTemplate.IsActive,
            emailTemplate.CreatedAt,
            emailTemplate.UpdatedAt);
    }
}
