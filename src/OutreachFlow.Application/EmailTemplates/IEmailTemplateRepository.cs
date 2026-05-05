using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Application.EmailTemplates;

public interface IEmailTemplateRepository
{
    Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default);

    Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EmailTemplate>> ListAsync(bool? activeOnly, CancellationToken cancellationToken = default);

    void Remove(EmailTemplate emailTemplate);
}
