using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Templates;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class SenderProfilesAndTemplatesEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldCreateDefaultSenderProfileAndClearPreviousDefault()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var firstProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            CreateSenderProfileRequest("Primary", "primary@example.com", true));

        var secondProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            CreateSenderProfileRequest("Secondary", "secondary@example.com", true));

        var profiles = await GetAsync<IReadOnlyList<SenderProfileDto>>(client, "/api/v1/sender-profiles");
        profiles.Single(profile => profile.Id == firstProfile.Id).IsDefault.Should().BeFalse();
        profiles.Single(profile => profile.Id == secondProfile.Id).IsDefault.Should().BeTrue();

        var defaultProfile = await GetAsync<SenderProfileDto>(client, "/api/v1/sender-profiles/default");
        defaultProfile.Id.Should().Be(secondProfile.Id);
    }

    [Fact]
    public async Task ShouldUpdateAndDeactivateSenderProfile()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var profile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            CreateSenderProfileRequest("Primary", "primary@example.com", false));

        var updatedProfile = await PutAsync<SenderProfileDto>(
            client,
            $"/api/v1/sender-profiles/{profile.Id}",
            new UpdateSenderProfileRequest(
                "Primary updated",
                "updated@example.com",
                null,
                "Northwind Studio",
                null,
                "<p>Best regards</p>",
                true,
                true,
                SenderSignatureFormat.Html));
        updatedProfile.Name.Should().Be("Primary updated");
        updatedProfile.IsDefault.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/v1/sender-profiles/{profile.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deactivatedProfile = await GetAsync<SenderProfileDto>(
            client,
            $"/api/v1/sender-profiles/{profile.Id}");
        deactivatedProfile.IsActive.Should().BeFalse();
        deactivatedProfile.IsDefault.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldCreateUpdateDeactivateAndFilterEmailTemplates()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                "Initial outreach",
                "Hello {{contact.displayName}}",
                "Hello {{contact.displayName}},\n\n{{sender.signature}}"));

        var updatedTemplate = await PutAsync<EmailTemplateDto>(
            client,
            $"/api/v1/templates/{template.Id}",
            new UpdateEmailTemplateRequest(
                "Intro updated",
                "Updated",
                "Updated subject",
                "Updated body",
                true));
        updatedTemplate.Name.Should().Be("Intro updated");

        var activeTemplates = await GetAsync<IReadOnlyList<EmailTemplateDto>>(
            client,
            "/api/v1/templates?activeOnly=true");
        activeTemplates.Should().ContainSingle(item => item.Id == template.Id);

        var deleteResponse = await client.DeleteAsync($"/api/v1/templates/{template.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activeTemplatesAfterDeactivate = await GetAsync<IReadOnlyList<EmailTemplateDto>>(
            client,
            "/api/v1/templates?activeOnly=true");
        activeTemplatesAfterDeactivate.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnSupportedTemplateVariables()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        var variables = await GetAsync<IReadOnlyList<TemplateVariableDto>>(
            client,
            "/api/v1/templates/variables");

        variables.Should().Contain(variable => variable.Name == "contact.displayName");
        variables.Should().Contain(variable => variable.Name == "sender.signature");
    }

    [Fact]
    public async Task ShouldRejectSignatureContentWithoutFormat()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/sender-profiles",
            new CreateSenderProfileRequest(
                "Primary sender",
                "sender@example.com",
                null,
                null,
                null,
                "<p>Best regards</p>",
                true));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await ReadAsync<ApiErrorResponse>(response);
        error.ErrorCode.Should().Be("VALIDATION_ERROR");
        error.Message.Should().Be("Signature format is required when signature content is provided.");
    }

    [Fact]
    public async Task ShouldRejectUnsupportedSignatureFormat()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/sender-profiles",
            new
            {
                name = "Primary sender",
                email = "sender@example.com",
                isDefault = true,
                signature = "<p>Best regards</p>",
                signatureFormat = 999
            },
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await ReadAsync<ApiErrorResponse>(response);
        error.ErrorCode.Should().Be("VALIDATION_ERROR");
        error.Message.Should().Be("Signature format is not supported.");
    }

    private static CreateSenderProfileRequest CreateSenderProfileRequest(
        string name,
        string email,
        bool isDefault)
    {
        return new CreateSenderProfileRequest(
            name,
            email,
            null,
            "Northwind Studio",
            "https://example.com",
            "<p>Best regards</p>",
            isDefault,
            SenderSignatureFormat.Html);
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
