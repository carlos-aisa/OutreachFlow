using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Domain.EmailDrafts;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class EmailDraftRepository(OutreachFlowDbContext dbContext) : IEmailDraftRepository
{
    public async Task AddRangeAsync(IReadOnlyList<EmailDraft> drafts, CancellationToken cancellationToken = default)
    {
        await dbContext.EmailDrafts.AddRangeAsync(drafts, cancellationToken);
    }

    public async Task<EmailDraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.EmailDrafts
            .Include(emailDraft => emailDraft.Attachments)
            .FirstOrDefaultAsync(emailDraft => emailDraft.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EmailDraft>> ListAsync(
        EmailDraftFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.EmailDrafts
            .Include(emailDraft => emailDraft.Attachments)
            .AsQueryable();

        if (filter.Status is not null)
        {
            query = query.Where(emailDraft => emailDraft.Status == filter.Status);
        }

        if (filter.ContactId is not null)
        {
            query = query.Where(emailDraft => emailDraft.ContactId == filter.ContactId);
        }

        var drafts = await query.ToArrayAsync(cancellationToken);

        return drafts
            .OrderByDescending(emailDraft => emailDraft.CreatedAt)
            .ToArray();
    }
}
