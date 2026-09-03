using System.Net.Http.Json;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Domain.ContactGroups;
using OutreachFlow.Web.Common;

namespace OutreachFlow.Web.ContactGroups;

public sealed class ContactGroupApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<ContactGroupDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/v1/contact-groups", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ContactGroupDto>>(response, cancellationToken);
    }

    public async Task<ContactGroupDto> CreateAsync(CreateContactGroupRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/v1/contact-groups", request, ApiClientJson.Options, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<ContactGroupDto>(response, cancellationToken);
    }

    public async Task<ContactGroupDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/contact-groups/{id}", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<ContactGroupDto>(response, cancellationToken);
    }

    public async Task<ContactGroupDto> UpdateAsync(Guid id, UpdateContactGroupRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync($"api/v1/contact-groups/{id}", request, ApiClientJson.Options, cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<ContactGroupDto>(response, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/contact-groups/{id}", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<IReadOnlyList<ContactGroupDto>> ListForContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/contacts/{contactId}/groups", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ContactGroupDto>>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<ContactGroupMemberDto>> ListMembersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/contact-groups/{id}/members", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ContactGroupMemberDto>>(response, cancellationToken);
    }

    public async Task<IReadOnlyList<ContactGroupMembershipDto>> ListMembershipStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/v1/contact-groups/{id}/membership-status", cancellationToken);
        return await ApiClientJson.ReadRequiredAsync<IReadOnlyList<ContactGroupMembershipDto>>(response, cancellationToken);
    }

    public async Task SetOverrideAsync(Guid id, Guid contactId, ContactGroupOverrideType type, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync($"api/v1/contact-groups/{id}/members/{contactId}/membership-override?type={type}", null, cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task ClearOverrideAsync(Guid id, Guid contactId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync($"api/v1/contact-groups/{id}/members/{contactId}/membership-override", cancellationToken);
        await ApiClientJson.EnsureSuccessAsync(response, cancellationToken);
    }
}
