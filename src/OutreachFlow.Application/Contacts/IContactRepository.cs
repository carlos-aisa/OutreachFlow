using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Application.Contacts;

public interface IContactRepository
{
    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);

    Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Contact?> GetByNormalizedEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Contact>> ListAsync(ContactFilterRequest filter, CancellationToken cancellationToken = default);

    void Remove(Contact contact);
}
