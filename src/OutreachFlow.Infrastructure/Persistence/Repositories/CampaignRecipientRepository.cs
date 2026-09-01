using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class CampaignRecipientRepository(OutreachFlowDbContext dbContext) : ICampaignRecipientRepository
{
    public async Task AddAsync(CampaignRecipient recipient, CancellationToken cancellationToken = default) =>
        await dbContext.CampaignRecipients.AddAsync(recipient, cancellationToken);

    public Task<CampaignRecipient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.CampaignRecipients.FirstOrDefaultAsync(recipient => recipient.Id == id, cancellationToken);

    public Task<CampaignRecipient?> GetByCampaignAndContactAsync(
        Guid campaignId,
        Guid contactId,
        Guid messageTemplateId,
        CancellationToken cancellationToken = default) =>
        dbContext.CampaignRecipients.FirstOrDefaultAsync(
            recipient => recipient.CampaignId == campaignId
                && recipient.ContactId == contactId
                && recipient.MessageTemplateId == messageTemplateId,
            cancellationToken);

    public Task<CampaignRecipient?> GetByEmailDraftIdAsync(Guid emailDraftId, CancellationToken cancellationToken = default) =>
        dbContext.CampaignRecipients.FirstOrDefaultAsync(recipient => recipient.EmailDraftId == emailDraftId, cancellationToken);

    public async Task<IReadOnlyList<CampaignRecipient>> ListByCampaignAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        var recipients = await dbContext.CampaignRecipients
            .Where(recipient => recipient.CampaignId == campaignId)
            .ToListAsync(cancellationToken);

        return recipients.OrderBy(recipient => recipient.IncorporatedAt).ToArray();
    }

    public async Task<IReadOnlyList<CampaignRecipient>> ListIncorporatedByCampaignAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default)
    {
        var recipients = await dbContext.CampaignRecipients
            .Where(recipient => recipient.CampaignId == campaignId && recipient.Status == CampaignRecipientStatus.Incorporated)
            .ToListAsync(cancellationToken);

        return recipients.OrderBy(recipient => recipient.IncorporatedAt).ToArray();
    }

    public async Task<IReadOnlyList<Guid>> ListContactIdsByCampaignAsync(
        Guid campaignId,
        CancellationToken cancellationToken = default) =>
        await dbContext.CampaignRecipients
            .Where(recipient => recipient.CampaignId == campaignId)
            .Select(recipient => recipient.ContactId)
            .Distinct()
            .ToArrayAsync(cancellationToken);
}
