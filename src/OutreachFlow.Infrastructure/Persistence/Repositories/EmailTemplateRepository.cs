using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class EmailTemplateRepository(OutreachFlowDbContext dbContext) : IEmailTemplateRepository
{
    public async Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default)
    {
        await dbContext.EmailTemplates.AddAsync(emailTemplate, cancellationToken);
    }

    public async Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.EmailTemplates
            .Include(emailTemplate => emailTemplate.DefaultAttachments)
            .FirstOrDefaultAsync(emailTemplate => emailTemplate.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EmailTemplate>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.EmailTemplates.AsQueryable();

        if (activeOnly is not null)
        {
            query = query.Where(emailTemplate => emailTemplate.IsActive == activeOnly);
        }

        return await query
            .Include(emailTemplate => emailTemplate.DefaultAttachments)
            .OrderBy(emailTemplate => emailTemplate.Name)
            .ToArrayAsync(cancellationToken);
    }

    public void Remove(EmailTemplate emailTemplate)
    {
        dbContext.EmailTemplates.Remove(emailTemplate);
    }
}
