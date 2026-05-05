using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class CoreContactsEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldCreateUpdateListAndDeleteOrganization()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var organization = await PostAsync<OrganizationDto>(
            client,
            "/api/v1/organizations",
            new CreateOrganizationRequest("Northwind Studio", null, null, null, null, null, null));

        var fetchedOrganization = await GetAsync<OrganizationDto>(
            client,
            $"/api/v1/organizations/{organization.Id}");
        fetchedOrganization.Name.Should().Be("Northwind Studio");

        var updatedOrganization = await PutAsync<OrganizationDto>(
            client,
            $"/api/v1/organizations/{organization.Id}",
            new UpdateOrganizationRequest(
                "Northwind Consulting",
                "Partner",
                null,
                "Valencia",
                null,
                "Spain",
                "Updated notes."));
        updatedOrganization.Name.Should().Be("Northwind Consulting");
        updatedOrganization.City.Should().Be("Valencia");

        var organizations = await GetAsync<IReadOnlyList<OrganizationDto>>(
            client,
            "/api/v1/organizations");
        organizations.Should().ContainSingle(item => item.Id == organization.Id);

        var deleteResponse = await client.DeleteAsync($"/api/v1/organizations/{organization.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var missingResponse = await client.GetAsync($"/api/v1/organizations/{organization.Id}");
        missingResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldCreateUpdateListAndRejectDuplicateTag()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var tag = await PostAsync<TagDto>(
            client,
            "/api/v1/tags",
            new CreateTagRequest("Priority", "Workflow"));

        var updatedTag = await PutAsync<TagDto>(
            client,
            $"/api/v1/tags/{tag.Id}",
            new UpdateTagRequest("Partner", "Relationship"));
        updatedTag.Name.Should().Be("Partner");
        updatedTag.Category.Should().Be("Relationship");

        var tags = await GetAsync<IReadOnlyList<TagDto>>(client, "/api/v1/tags");
        tags.Should().ContainSingle(item => item.Id == tag.Id);

        var duplicateResponse = await client.PostAsJsonAsync(
            "/api/v1/tags",
            new CreateTagRequest(" partner ", " relationship "),
            JsonOptions);
        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await ReadAsync<ApiErrorResponse>(duplicateResponse);
        error.ErrorCode.Should().Be("CONFLICT");
    }

    [Fact]
    public async Task ShouldCreateFilterAssignAndRemoveContactTag()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var organization = await PostAsync<OrganizationDto>(
            client,
            "/api/v1/organizations",
            new CreateOrganizationRequest("Northwind Studio", null, null, null, null, null, null));
        var tag = await PostAsync<TagDto>(
            client,
            "/api/v1/tags",
            new CreateTagRequest("Priority", null));
        var matchingContact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                organization.Id,
                "Alex Morgan",
                "alex@example.com",
                null,
                "Operations Manager",
                "Manual",
                ContactStatus.New,
                false));
        await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                organization.Id,
                "Sam Taylor",
                "sam@example.com",
                null,
                null,
                null,
                ContactStatus.Contacted,
                false));

        var assignResponse = await client.PostAsync(
            $"/api/v1/contacts/{matchingContact.Id}/tags/{tag.Id}",
            content: null);
        assignResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var contacts = await GetAsync<IReadOnlyList<ContactDto>>(
            client,
            $"/api/v1/contacts?search=Alex&tagId={tag.Id}&status=New&doNotContact=false&organizationId={organization.Id}");
        contacts.Should().ContainSingle()
            .Which.Tags.Should().ContainSingle(contactTag => contactTag.Id == tag.Id);

        var removeResponse = await client.DeleteAsync(
            $"/api/v1/contacts/{matchingContact.Id}/tags/{tag.Id}");
        removeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var untaggedContact = await GetAsync<ContactDto>(client, $"/api/v1/contacts/{matchingContact.Id}");
        untaggedContact.Tags.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnConflictWhenCreatingDuplicateContactEmail()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                null,
                "Alex Morgan",
                "alex@example.com",
                null,
                null,
                null,
                ContactStatus.New,
                false));

        var duplicateResponse = await client.PostAsJsonAsync(
            "/api/v1/contacts",
            new CreateContactRequest(
                null,
                "Avery Lee",
                " ALEX@example.com ",
                null,
                null,
                null,
                ContactStatus.New,
                false),
            JsonOptions);

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await ReadAsync<ApiErrorResponse>(duplicateResponse);
        error.Message.Should().Be("A contact with this email already exists.");
    }

    private static async Task<T> GetAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string uri, object request)
    {
        using var response = await client.PostAsJsonAsync(uri, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PutAsync<T>(HttpClient client, string uri, object request)
    {
        using var response = await client.PutAsJsonAsync(uri, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return result ?? throw new InvalidOperationException("The API returned an empty response.");
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    private sealed record ApiErrorResponse(string ErrorCode, string Message);
}
