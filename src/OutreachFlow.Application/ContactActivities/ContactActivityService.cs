using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.ContactActivities;

public sealed class ContactActivityService(
    IContactRepository contactRepository,
    IContactActivityRepository contactActivityRepository) : IContactActivityService
{
    public async Task RecordAsync(
        CreateContactActivityRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var activity = ContactActivity.Create(
                request.ContactId,
                request.OrganizationId,
                request.Type,
                request.Subject,
                request.BodyPreview,
                request.MetadataJson,
                request.OccurredAt ?? DateTimeOffset.UtcNow);

            await contactActivityRepository.AddAsync(activity, cancellationToken);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }

    public async Task<IReadOnlyList<ContactActivityDto>> ListByContactIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        _ = await contactRepository.GetByIdAsync(contactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Contact was not found.");

        var activities = await contactActivityRepository.ListByContactIdAsync(contactId, cancellationToken);

        return activities.Select(activity => new ContactActivityDto(
            activity.Id,
            activity.ContactId,
            activity.OrganizationId,
            activity.Type,
            activity.Subject,
            activity.BodyPreview,
            activity.MetadataJson,
            activity.OccurredAt))
            .ToArray();
    }
}
