namespace OutreachFlow.Application.Contacts;

public interface IContactLookupService
{
    Task<ContactDto> MapAsync(
        OutreachFlow.Domain.Contacts.Contact contact,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactDto>> MapAsync(
        IReadOnlyList<OutreachFlow.Domain.Contacts.Contact> contacts,
        CancellationToken cancellationToken = default);
}
