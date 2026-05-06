using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class EmailDraftSendingEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldSendApprovedDraftAndUpdateContactLastContacted()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var contact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                OrganizationId: null,
                DisplayName: "Alex Morgan",
                Email: "alex@example.com",
                Phone: null,
                Role: "Manager",
                Source: "Manual",
                Status: ContactStatus.New,
                DoNotContact: false));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            BuildSenderProfileRequest());
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));
        var draft = await GenerateSingleDraftAsync(client, template.Id, senderProfile.Id);
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draft.Id}/approve");

        var sentDraft = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draft.Id}/send");

        sentDraft.Status.Should().Be(EmailDraftStatus.Sent);
        sentDraft.SentAt.Should().NotBeNull();
        var updatedContact = await GetAsync<ContactDto>(client, $"/api/v1/contacts/{contact.Id}");
        updatedContact.LastContactedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldRejectSendWhenDraftIsNotApproved()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        _ = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                OrganizationId: null,
                DisplayName: "Alex Morgan",
                Email: "alex@example.com",
                Phone: null,
                Role: "Manager",
                Source: "Manual",
                Status: ContactStatus.New,
                DoNotContact: false));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            BuildSenderProfileRequest());
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));
        var draft = await GenerateSingleDraftAsync(client, template.Id, senderProfile.Id);

        using var response = await client.PostAsync($"/api/v1/drafts/{draft.Id}/send", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await ReadAsync<ApiErrorResponse>(response);
        error.ErrorCode.Should().Be("VALIDATION_ERROR");
        error.Message.Should().Be("Only approved drafts can be sent.");
    }

    [Fact]
    public async Task ShouldRejectDuplicateSendForSameDraft()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        _ = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                OrganizationId: null,
                DisplayName: "Alex Morgan",
                Email: "alex@example.com",
                Phone: null,
                Role: "Manager",
                Source: "Manual",
                Status: ContactStatus.New,
                DoNotContact: false));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            BuildSenderProfileRequest());
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));
        var draft = await GenerateSingleDraftAsync(client, template.Id, senderProfile.Id);
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draft.Id}/approve");
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draft.Id}/send");

        using var duplicateResponse = await client.PostAsync($"/api/v1/drafts/{draft.Id}/send", null);

        duplicateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await ReadAsync<ApiErrorResponse>(duplicateResponse);
        error.Message.Should().Be("This draft was already sent.");
    }

    [Fact]
    public async Task ShouldRejectEquivalentRecentEmailSend()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        _ = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                OrganizationId: null,
                DisplayName: "Alex Morgan",
                Email: "alex@example.com",
                Phone: null,
                Role: "Manager",
                Source: "Manual",
                Status: ContactStatus.New,
                DoNotContact: false));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            BuildSenderProfileRequest());
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));
        var firstDraft = await GenerateSingleDraftAsync(client, template.Id, senderProfile.Id);
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{firstDraft.Id}/approve");
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{firstDraft.Id}/send");

        var secondDraft = await GenerateSingleDraftAsync(client, template.Id, senderProfile.Id);
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{secondDraft.Id}/approve");

        using var response = await client.PostAsync($"/api/v1/drafts/{secondDraft.Id}/send", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await ReadAsync<ApiErrorResponse>(response);
        error.Message.Should().Be("An equivalent email was already sent to this contact recently.");
    }

    [Fact]
    public async Task ShouldPersistFailedSendWhenFakeSenderIsTriggered()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        _ = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                OrganizationId: null,
                DisplayName: "Alex Morgan",
                Email: "alex@example.com",
                Phone: null,
                Role: "Manager",
                Source: "Manual",
                Status: ContactStatus.New,
                DoNotContact: false));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            BuildSenderProfileRequest());
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Failing template",
                null,
                "Subject [fail-send]",
                "Body"));
        var draft = await GenerateSingleDraftAsync(client, template.Id, senderProfile.Id);
        _ = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draft.Id}/approve");

        var failedDraft = await PostWithoutBodyAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draft.Id}/send");

        failedDraft.Status.Should().Be(EmailDraftStatus.Failed);
        failedDraft.FailureReason.Should().Be("Simulated failure from fake email sender.");
    }

    private static async Task<EmailDraftDto> GenerateSingleDraftAsync(
        HttpClient client,
        Guid templateId,
        Guid senderProfileId)
    {
        var result = await PostAsync<GenerateEmailDraftsResult>(
            client,
            "/api/v1/drafts/generate",
            new GenerateEmailDraftsRequest(
                Search: null,
                TagId: null,
                Status: null,
                DoNotContact: null,
                OrganizationId: null,
                LastContactedFrom: null,
                LastContactedTo: null,
                TemplateId: templateId,
                SenderProfileId: senderProfileId,
                AttachmentAssetIds: []));

        result.Drafts.Should().ContainSingle();
        return result.Drafts.Single();
    }

    private static CreateSenderProfileRequest BuildSenderProfileRequest()
    {
        return new CreateSenderProfileRequest(
            "Primary sender",
            "sender@example.com",
            null,
            null,
            null,
            "<p>Best regards</p>",
            true,
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

    private static async Task<T> PostWithoutBodyAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.PostAsync(uri, null);
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
