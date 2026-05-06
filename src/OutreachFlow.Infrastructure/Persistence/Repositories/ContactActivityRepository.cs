using Microsoft.EntityFrameworkCore;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Infrastructure.Persistence.Repositories;

public sealed class ContactActivityRepository(OutreachFlowDbContext dbContext) : IContactActivityRepository
{
    public async Task AddAsync(ContactActivity activity, CancellationToken cancellationToken = default)
    {
        await dbContext.ContactActivities.AddAsync(activity, cancellationToken);
    }

    public async Task<IReadOnlyList<ContactActivity>> ListByContactIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        var activities = await dbContext.ContactActivities
            .Where(activity => activity.ContactId == contactId)
            .ToArrayAsync(cancellationToken);

        return activities
            .OrderByDescending(activity => activity.OccurredAt)
            .ToArray();
    }
}
