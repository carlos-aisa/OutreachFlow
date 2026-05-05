using OutreachFlow.Domain.EmailMessages;

namespace OutreachFlow.Application.EmailSending;

public interface IEmailMessageRepository
{
    Task AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);

    Task<bool> ExistsEquivalentSentEmailAsync(
        Guid contactId,
        string subject,
        DateTimeOffset since,
        CancellationToken cancellationToken = default);
}
