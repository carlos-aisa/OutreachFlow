using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class EmailDraftGenerationEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldGenerateDraftsAndReportSkippedContacts()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var eligibleContact = await PostAsync<ContactDto>(
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
        _ = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(
                OrganizationId: null,
                DisplayName: "Jamie Smith",
                Email: "jamie@example.com",
                Phone: null,
                Role: "Director",
                Source: "Manual",
                Status: ContactStatus.New,
                DoNotContact: true));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            new CreateSenderProfileRequest(
                "Primary sender",
                "sender@example.com",
                null,
                "Northwind Studio",
                null,
                "<p>Best regards</p>",
                true,
                SenderSignatureFormat.Html));
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject for {{contact.displayName}}",
                "Hello {{contact.displayName}}, organization {{organization.name}}."));
        var defaultAttachment = await UploadAttachmentAsync(client, "Brochure", "brochure.pdf");
        var optionalAttachment = await UploadAttachmentAsync(client, "Proposal", "proposal.pdf");
        _ = await PostWithoutBodyAsync<EmailTemplateDto>(
            client,
            $"/api/v1/templates/{template.Id}/attachments/{defaultAttachment.Id}");

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
                AttachmentAssetIds: [optionalAttachment.Id]));

        generationResult.RequestedContacts.Should().Be(2);
        generationResult.GeneratedDrafts.Should().Be(1);
        generationResult.SkippedContacts.Should().Be(1);
        generationResult.Drafts.Should().ContainSingle();
        var generatedDraft = generationResult.Drafts.Single();
        generatedDraft.ContactId.Should().Be(eligibleContact.Id);
        generatedDraft.Status.Should().Be(EmailDraftStatus.NeedsReview);
        generatedDraft.MissingVariables.Should().Contain("organization.name");
        generatedDraft.AttachmentAssetIds.Should().BeEquivalentTo([defaultAttachment.Id, optionalAttachment.Id]);
        generationResult.Skipped.Should().ContainSingle(item => item.Reason.Contains("Do Not Contact"));
    }

    [Fact]
    public async Task ShouldListAndGetGeneratedDrafts()
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
                "<p>Best regards</p>",
                true,
                SenderSignatureFormat.Html));
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));

        _ = await PostAsync<GenerateEmailDraftsResult>(
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

        var drafts = await GetAsync<IReadOnlyList<EmailDraftDto>>(client, "/api/v1/drafts");
        drafts.Should().ContainSingle();

        var draft = await GetAsync<EmailDraftDto>(client, $"/api/v1/drafts/{drafts[0].Id}");
        draft.Id.Should().Be(drafts[0].Id);
    }

    private static async Task<AttachmentAssetDto> UploadAttachmentAsync(
        HttpClient client,
        string name,
        string fileName)
    {
        using var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "name" }
        };

        using var fileContent = new ByteArrayContent([1, 2, 3, 4]);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        formData.Add(fileContent, "file", fileName);

        using var response = await client.PostAsync("/api/v1/attachments", formData);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<AttachmentAssetDto>(response);
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
}
