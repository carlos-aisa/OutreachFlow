namespace OutreachFlow.Application.Contacts;

public interface IContactService
{
    Task<ContactDto> CreateAsync(CreateContactRequest request, CancellationToken cancellationToken = default);

    Task<ContactDto> UpdateAsync(Guid id, UpdateContactRequest request, CancellationToken cancellationToken = default);

    Task<ContactDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactDto>> ListAsync(ContactFilterRequest filter, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task AssignTagAsync(Guid contactId, Guid tagId, CancellationToken cancellationToken = default);

    Task RemoveTagAsync(Guid contactId, Guid tagId, CancellationToken cancellationToken = default);
}
