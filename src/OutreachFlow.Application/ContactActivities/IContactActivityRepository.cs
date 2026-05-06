using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.ContactActivities;

public interface IContactActivityRepository
{
    Task AddAsync(ContactActivity activity, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactActivity>> ListByContactIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default);
}
