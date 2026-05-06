using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class LocalizationEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ShouldReturnSpanishValidationMessageFromAcceptLanguage()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        await CreateContactAsync(client, "alex@example.com");

        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("es-ES"));

        using var response = await client.PostAsJsonAsync(
            "/api/v1/contacts",
            BuildCreateContactRequest("alex@example.com"),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
        error.Should().NotBeNull();
        error!.Message.Should().Be("Ya existe un contacto con ese correo electrónico.");
    }

    [Fact]
    public async Task ShouldPrioritizeExplicitCultureQueryOverHeaders()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        await CreateContactAsync(client, "alex@example.com");

        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));

        using var response = await client.PostAsJsonAsync(
            "/api/v1/contacts?culture=es-ES",
            BuildCreateContactRequest("alex@example.com"),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
        error.Should().NotBeNull();
        error!.Message.Should().Be("Ya existe un contacto con ese correo electrónico.");
    }

    [Fact]
    public async Task ShouldFallbackToDefaultCultureWhenLanguageIsUnsupported()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        await CreateContactAsync(client, "alex@example.com");

        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("fr-FR"));

        using var response = await client.PostAsJsonAsync(
            "/api/v1/contacts",
            BuildCreateContactRequest("alex@example.com"),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
        error.Should().NotBeNull();
        error!.Message.Should().Be("A contact with this email already exists.");
    }

    private static async Task CreateContactAsync(HttpClient client, string email)
    {
        using var response = await client.PostAsJsonAsync(
            "/api/v1/contacts",
            BuildCreateContactRequest(email),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private static CreateContactRequest BuildCreateContactRequest(string email)
    {
        return new CreateContactRequest(
            OrganizationId: null,
            DisplayName: "Alex Morgan",
            Email: email,
            Phone: null,
            Role: "Manager",
            Source: "Manual",
            Status: ContactStatus.New,
            DoNotContact: false);
    }

    private sealed record ApiErrorResponse(string ErrorCode, string Message);
}
