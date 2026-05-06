namespace OutreachFlow.Application.ContactActivities;

public interface IContactActivityService
{
    Task RecordAsync(
        CreateContactActivityRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactActivityDto>> ListByContactIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default);
}
