using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Domain.EmailMessages;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class EmailMessageRepository(OutreachFlowDbContext dbContext) : IEmailMessageRepository
{
    public async Task AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        await dbContext.EmailMessages.AddAsync(emailMessage, cancellationToken);
    }

    public Task<bool> ExistsEquivalentSentEmailAsync(
        Guid contactId,
        string subject,
        DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        return ExistsEquivalentSentEmailCoreAsync(contactId, subject, since, cancellationToken);
    }

    private async Task<bool> ExistsEquivalentSentEmailCoreAsync(
        Guid contactId,
        string subject,
        DateTimeOffset since,
        CancellationToken cancellationToken)
    {
        var sentMessages = await dbContext.EmailMessages
            .Where(emailMessage =>
                emailMessage.ContactId == contactId &&
                emailMessage.Status == EmailMessageStatus.Sent &&
                emailMessage.Subject == subject)
            .ToArrayAsync(cancellationToken);

        return sentMessages.Any(emailMessage => emailMessage.CreatedAt >= since);
    }
}
