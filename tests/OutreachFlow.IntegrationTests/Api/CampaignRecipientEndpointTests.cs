using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class CampaignRecipientEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldDiscoverIncorporateAndListRecipients()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Intro", null, "Hello", "Body"));
        var group = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));
        var contact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(null, "Alex Morgan", "alex@example.com", null, null, null, ContactStatus.New, false));
        var campaign = await PostAsync<CampaignDto>(
            client,
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id], false, 7, FollowUpTaskType.Email));

        var candidates = await GetAsync<IReadOnlyList<CampaignRecipientCandidateDto>>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/candidates");
        candidates.Should().ContainSingle(candidate => candidate.ContactId == contact.Id);

        var recipient = await PostAsync<CampaignRecipientDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients/{contact.Id}",
            request: null);
        recipient.Status.Should().Be(CampaignRecipientStatus.Incorporated);

        var recipients = await GetAsync<IReadOnlyList<CampaignRecipientDto>>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients");
        recipients.Should().ContainSingle(item => item.ContactId == contact.Id);

        var candidatesAfterIncorporation = await GetAsync<IReadOnlyList<CampaignRecipientCandidateDto>>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/candidates");
        candidatesAfterIncorporation.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldReturnConflictWhenIncorporatingSameContactTwice()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Intro", null, "Hello", "Body"));
        var group = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));
        var contact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(null, "Alex Morgan", "alex@example.com", null, null, null, ContactStatus.New, false));
        var campaign = await PostAsync<CampaignDto>(
            client,
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id], false, 7, FollowUpTaskType.Email));
        await PostAsync<CampaignRecipientDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients/{contact.Id}",
            request: null);

        using var response = await client.PostAsync($"/api/v1/campaigns/{campaign.Id}/recipients/{contact.Id}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ShouldGenerateDraftsForIncorporatedRecipients()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Intro", null, "Hello {{contact.displayName}}", "Body"));
        var group = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            new CreateSenderProfileRequest("Primary Sender", "sender@example.com", null, null, null, null, false));
        var contact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(null, "Alex Morgan", "alex@example.com", null, null, null, ContactStatus.New, false));
        var campaign = await PostAsync<CampaignDto>(
            client,
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id], false, 7, FollowUpTaskType.Email));
        await PostAsync<CampaignRecipientDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients/{contact.Id}",
            request: null);

        var result = await PostAsync<GenerateCampaignDraftsResult>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients/generate-drafts",
            new GenerateCampaignDraftsRequest(senderProfile.Id, []));

        result.RequestedRecipients.Should().Be(1);
        result.GeneratedDrafts.Should().Be(1);
        result.Recipients.Single().Status.Should().Be(CampaignRecipientStatus.Drafted);
    }

    [Fact]
    public async Task ShouldExcludeDoNotContactRecipientDuringDraftGeneration()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Intro", null, "Hello", "Body"));
        var group = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));
        var senderProfile = await PostAsync<SenderProfileDto>(
            client,
            "/api/v1/sender-profiles",
            new CreateSenderProfileRequest("Primary Sender", "sender@example.com", null, null, null, null, false));
        var contact = await PostAsync<ContactDto>(
            client,
            "/api/v1/contacts",
            new CreateContactRequest(null, "Jamie Smith", "jamie@example.com", null, null, null, ContactStatus.New, true));
        var campaign = await PostAsync<CampaignDto>(
            client,
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", null, template.Id, [group.Id], false, 7, FollowUpTaskType.Email));
        await PostAsync<CampaignRecipientDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients/{contact.Id}",
            request: null);

        var result = await PostAsync<GenerateCampaignDraftsResult>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients/generate-drafts",
            new GenerateCampaignDraftsRequest(senderProfile.Id, []));

        result.GeneratedDrafts.Should().Be(0);
        result.ExcludedRecipients.Should().Be(1);
        var recipients = await GetAsync<IReadOnlyList<CampaignRecipientDto>>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/recipients");
        recipients.Single().Status.Should().Be(CampaignRecipientStatus.Excluded);
        recipients.Single().ExclusionReason.Should().Be("Contact is marked as Do Not Contact.");
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownCampaignWhenListingRecipients()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/v1/campaigns/{Guid.NewGuid()}/recipients");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<T> GetAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string uri, object? request)
    {
        HttpResponseMessage response;

        if (request is null)
        {
            response = await client.PostAsync(uri, null);
        }
        else
        {
            response = await client.PostAsJsonAsync(uri, request, JsonOptions);
        }

        using (response)
        {
            response.EnsureSuccessStatusCode();
            return await ReadAsync<T>(response);
        }
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
