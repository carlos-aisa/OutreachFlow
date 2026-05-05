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

namespace OutreachFlow.IntegrationTests.Api;

public sealed class EmailDraftReviewEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldListUpdateApproveAndCancelDraft()
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
            new CreateSenderProfileRequest(
                "Primary sender",
                "sender@example.com",
                null,
                null,
                null,
                "Best regards",
                true));
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));
        var generationResult = await PostAsync<GenerateEmailDraftsResult>(
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
                TemplateId: template.Id,
                SenderProfileId: senderProfile.Id,
                AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;

        var drafts = await GetAsync<IReadOnlyList<EmailDraftDto>>(client, "/api/v1/drafts?status=Draft");
        drafts.Should().ContainSingle(item => item.Id == draftId);

        var draft = await GetAsync<EmailDraftDto>(client, $"/api/v1/drafts/{draftId}");
        draft.Id.Should().Be(draftId);

        var updated = await PutAsync<EmailDraftDto>(
            client,
            $"/api/v1/drafts/{draftId}",
            new UpdateEmailDraftRequest(
                "Updated subject",
                "Updated body"));
        updated.Status.Should().Be(EmailDraftStatus.Draft);
        updated.Subject.Should().Be("Updated subject");
        updated.Body.Should().Be("Updated body");

        var approved = await PostWithoutBodyAsync<EmailDraftDto>(
            client,
            $"/api/v1/drafts/{draftId}/approve");
        approved.Status.Should().Be(EmailDraftStatus.Approved);
        approved.ApprovedAt.Should().NotBeNull();

        var cancelled = await PostWithoutBodyAsync<EmailDraftDto>(
            client,
            $"/api/v1/drafts/{draftId}/cancel");
        cancelled.Status.Should().Be(EmailDraftStatus.Cancelled);
        cancelled.CancelledAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldRejectApprovingDraftWithUnresolvedVariables()
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
            new CreateSenderProfileRequest(
                "Primary sender",
                "sender@example.com",
                null,
                null,
                null,
                "Best regards",
                true));
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject {{contact.displayName}}",
                "Body {{organization.name}}"));
        var generationResult = await PostAsync<GenerateEmailDraftsResult>(
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
                TemplateId: template.Id,
                SenderProfileId: senderProfile.Id,
                AttachmentAssetIds: []));
        var draftId = generationResult.Drafts.Single().Id;

        using var response = await client.PostAsync($"/api/v1/drafts/{draftId}/approve", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await ReadAsync<ApiErrorResponse>(response);
        error.ErrorCode.Should().Be("VALIDATION_ERROR");
        error.Message.Should().Be("Draft cannot be approved while render errors remain.");
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
