using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Domain.Campaigns;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class CampaignEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldCreateListAndUpdateCampaign()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Intro", null, "Hello", "Body"));
        var otherTemplate = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Follow-up", null, "Following up", "Body"));
        var group = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));

        var campaign = await PostAsync<CampaignDto>(
            client,
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", "Reach new prospects", template.Id, [group.Id]));

        campaign.Status.Should().Be(CampaignStatus.Open);
        campaign.AudienceGroupIds.Should().ContainSingle().Which.Should().Be(group.Id);

        var campaigns = await GetAsync<IReadOnlyList<CampaignDto>>(client, "/api/v1/campaigns");
        campaigns.Should().ContainSingle(item => item.Id == campaign.Id);

        var updated = await PutAsync<CampaignDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}",
            new UpdateCampaignRequest("Winter outreach", "Updated purpose", otherTemplate.Id));
        updated.Name.Should().Be("Winter outreach");
        updated.EmailTemplateId.Should().Be(otherTemplate.Id);
    }

    [Fact]
    public async Task ShouldManageAudienceGroupsAndLifecycle()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest("Intro", null, "Hello", "Body"));
        var firstGroup = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));
        var secondGroup = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Leads", []));
        var campaign = await PostAsync<CampaignDto>(
            client,
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", null, template.Id, [firstGroup.Id]));

        var withSecondGroup = await PostAsync<CampaignDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/audience-groups/{secondGroup.Id}",
            request: null);
        withSecondGroup.AudienceGroupIds.Should().HaveCount(2);

        var withFirstGroupOnly = await DeleteAsync<CampaignDto>(
            client,
            $"/api/v1/campaigns/{campaign.Id}/audience-groups/{secondGroup.Id}");
        withFirstGroupOnly.AudienceGroupIds.Should().ContainSingle().Which.Should().Be(firstGroup.Id);

        var closed = await PostAsync<CampaignDto>(client, $"/api/v1/campaigns/{campaign.Id}/close", request: null);
        closed.Status.Should().Be(CampaignStatus.Closed);

        var reopened = await PostAsync<CampaignDto>(client, $"/api/v1/campaigns/{campaign.Id}/reopen", request: null);
        reopened.Status.Should().Be(CampaignStatus.Open);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownCampaign()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.GetAsync($"/api/v1/campaigns/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldReturnBadRequestWhenTemplateDoesNotExist()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var group = await PostAsync<ContactGroupDto>(
            client,
            "/api/v1/contact-groups",
            new CreateContactGroupRequest("Prospects", []));

        using var response = await client.PostAsJsonAsync(
            "/api/v1/campaigns",
            new CreateCampaignRequest("Autumn outreach", null, Guid.NewGuid(), [group.Id]),
            JsonOptions);

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

    private static async Task<T> PutAsync<T>(HttpClient client, string uri, object request)
    {
        using var response = await client.PutAsJsonAsync(uri, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> DeleteAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.DeleteAsync(uri);
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
