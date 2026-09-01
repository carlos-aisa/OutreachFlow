using System.Net;
using System.Text;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Campaigns;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.ContactGroups;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.EmailDrafts;
using OutreachFlow.Web.FollowUps;
using OutreachFlow.Web.Organizations;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class HomeComponentTests : BunitContext
{
    private static readonly Guid CampaignId = Guid.Parse("6a1b2c3d-4e5f-4a6b-8c7d-9e0f1a2b3c4d");
    private static readonly Guid SecondCampaignId = Guid.Parse("9d0e1f2a-3b4c-4d5e-8f6a-1b2c3d4e5f6a");
    private static readonly Guid TemplateId = Guid.Parse("7b2c3d4e-5f60-4a7b-8c9d-0e1f2a3b4c5d");
    private static readonly Guid SenderProfileId = Guid.Parse("8c3d4e5f-6071-4b7c-8d9e-0f1a2b3c4d5e");

    [Fact]
    public void ShouldShowEmptyStateWhenNothingIsPending()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderHome(new HomeHttpMessageHandler());

        component.WaitForAssertion(() =>
            component.Markup.Should().Contain("Nothing needs your attention right now."));

        component.FindAll(".queue-row").Should().BeEmpty();
        component.Markup.Should().NotContain("Active campaigns");
    }

    [Fact]
    public void ShouldRenderQueueItemsInFixedCategoryOrderWithCountsAndLinks()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        var handler = new HomeHttpMessageHandler
        {
            NeedsReviewDraftCount = 3,
            OverdueFollowUpCount = 4,
            DueTodayFollowUpCount = 3,
            CandidateCount = 5
        };
        using var component = RenderHome(handler);

        component.WaitForAssertion(() => component.FindAll(".queue-row").Should().HaveCount(3));

        var rows = component.FindAll(".queue-row");
        rows[0].TextContent.Should().Contain("3 drafts ready for review");
        rows[0].QuerySelector("a")!.GetAttribute("href").Should().Be("/drafts");

        rows[1].TextContent.Should().Contain("7 follow-ups need attention");
        rows[1].TextContent.Should().Contain("4 overdue, 3 due today");
        rows[1].QuerySelector("a")!.GetAttribute("href").Should().Be("/follow-ups");

        rows[2].TextContent.Should().Contain("5 new contacts match your active campaigns");
        rows[2].QuerySelector("a")!.GetAttribute("href").Should().Be($"/campaigns/{CampaignId}#campaign-candidates");
    }

    [Fact]
    public void ShouldHideActiveCampaignsSectionWhenNoCampaignData()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderHome(new HomeHttpMessageHandler { NeedsReviewDraftCount = 1 });

        component.WaitForAssertion(() => component.FindAll(".queue-row").Should().HaveCount(1));

        component.Markup.Should().NotContain("Active campaigns");
        component.FindAll(".campaign-card").Should().BeEmpty();
    }

    [Fact]
    public void ShouldRenderActiveCampaignCardWithStats()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        var handler = new HomeHttpMessageHandler
        {
            IncludeOpenCampaign = true,
            SentCount = 2,
            PendingCount = 1,
            CandidateCount = 0
        };
        using var component = RenderHome(handler);

        component.WaitForAssertion(() => component.FindAll(".campaign-card").Should().HaveCount(1));

        var card = component.Find(".campaign-card");
        card.TextContent.Should().Contain("Autumn Outreach");
        card.QuerySelectorAll(".campaign-stat-value").Select(value => value.TextContent).Should().Equal("2", "1", "0");
    }

    [Fact]
    public void ShouldLinkCandidatesQueueItemToTheCampaignWithTheMostCandidatesSection()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        var handler = new HomeHttpMessageHandler
        {
            CandidateCount = 2,
            SecondCampaignCandidateCount = 6
        };
        using var component = RenderHome(handler);

        component.WaitForAssertion(() => component.FindAll(".queue-row").Should().HaveCount(1));

        var row = component.Find(".queue-row");
        row.TextContent.Should().Contain("8 new contacts match your active campaigns");
        row.QuerySelector("a")!.GetAttribute("href").Should().Be($"/campaigns/{SecondCampaignId}#campaign-candidates");
    }

    private IRenderedComponent<Home> RenderHome(HomeHttpMessageHandler handler)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new EmailDraftApiClient(httpClient));
        Services.AddSingleton(new FollowUpTaskApiClient(httpClient));
        Services.AddSingleton(new CampaignApiClient(httpClient));
        Services.AddSingleton(new CampaignRecipientApiClient(httpClient));
        Services.AddSingleton(new ContactGroupApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));

        return Render<Home>();
    }

    private sealed class HomeHttpMessageHandler : HttpMessageHandler
    {
        public int NeedsReviewDraftCount { get; set; }

        public int OverdueFollowUpCount { get; set; }

        public int DueTodayFollowUpCount { get; set; }

        public int CandidateCount { get; set; }

        public int SecondCampaignCandidateCount { get; set; }

        public bool IncludeOpenCampaign { get; set; }

        public int SentCount { get; set; }

        public int PendingCount { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var hasSecondCampaign = SecondCampaignCandidateCount > 0;
            var hasOpenCampaign = IncludeOpenCampaign || CandidateCount > 0 || hasSecondCampaign;

            if (request.Method == HttpMethod.Get && path == "/api/v1/drafts")
            {
                return Task.FromResult(JsonResponse(BuildDrafts(NeedsReviewDraftCount)));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/follow-ups")
            {
                return Task.FromResult(JsonResponse(BuildFollowUps(OverdueFollowUpCount, DueTodayFollowUpCount)));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/contact-groups")
            {
                return Task.FromResult(JsonResponse("[]"));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/campaigns")
            {
                if (!hasOpenCampaign)
                {
                    return Task.FromResult(JsonResponse("[]"));
                }

                var campaigns = hasSecondCampaign
                    ? $"[{CampaignJson(CampaignId, "Autumn Outreach")},{CampaignJson(SecondCampaignId, "Winter Outreach")}]"
                    : $"[{CampaignJson(CampaignId, "Autumn Outreach")}]";
                return Task.FromResult(JsonResponse(campaigns));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/campaigns/{CampaignId}/recipients")
            {
                return Task.FromResult(JsonResponse(BuildRecipients(CampaignId, SentCount, PendingCount)));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/campaigns/{CampaignId}/candidates")
            {
                return Task.FromResult(JsonResponse(BuildCandidates(CandidateCount)));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/campaigns/{SecondCampaignId}/recipients")
            {
                return Task.FromResult(JsonResponse(BuildRecipients(SecondCampaignId, 0, 0)));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/campaigns/{SecondCampaignId}/candidates")
            {
                return Task.FromResult(JsonResponse(BuildCandidates(SecondCampaignCandidateCount)));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/contacts")
            {
                return Task.FromResult(JsonResponse("[]"));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/organizations")
            {
                return Task.FromResult(JsonResponse("[]"));
            }

            return Task.FromResult(JsonResponse("{}"));
        }

        private static string BuildDrafts(int count)
        {
            var now = DateTimeOffset.UtcNow;
            var items = Enumerable.Range(0, count).Select(index => $$"""
                {
                    "id": "{{Guid.NewGuid()}}",
                    "contactId": "{{Guid.NewGuid()}}",
                    "contactDisplayName": "Contact {{index}}",
                    "contactEmail": "contact{{index}}@example.com",
                    "organizationId": null,
                    "templateId": null,
                    "senderProfileId": "{{SenderProfileId}}",
                    "subject": "Subject {{index}}",
                    "body": "Body {{index}}",
                    "status": "NeedsReview",
                    "hasRenderErrors": false,
                    "missingVariables": [],
                    "unknownVariables": [],
                    "attachmentAssetIds": [],
                    "createdAt": "{{now:O}}",
                    "updatedAt": "{{now:O}}",
                    "approvedAt": null,
                    "sentAt": null,
                    "failureReason": null,
                    "cancelledAt": null
                }
                """);

            return $"[{string.Join(",", items)}]";
        }

        private static string BuildFollowUps(int overdueCount, int dueTodayCount)
        {
            var now = DateTimeOffset.Now;
            var overdue = Enumerable.Range(0, overdueCount).Select(_ => FollowUpJson(now.AddDays(-1)));
            var dueToday = Enumerable.Range(0, dueTodayCount).Select(_ => FollowUpJson(now.Date.AddHours(9)));

            return $"[{string.Join(",", overdue.Concat(dueToday))}]";
        }

        private static string FollowUpJson(DateTimeOffset dueAt) => $$"""
            {
                "id": "{{Guid.NewGuid()}}",
                "contactId": "{{Guid.NewGuid()}}",
                "contactDisplayName": "Contact",
                "contactEmail": "contact@example.com",
                "organizationId": null,
                "dueAt": "{{dueAt:O}}",
                "type": "Email",
                "notes": null,
                "isCompleted": false,
                "completedAt": null,
                "createdAt": "{{dueAt:O}}",
                "updatedAt": "{{dueAt:O}}"
            }
            """;

        private static string CampaignJson(Guid campaignId, string name) => $$"""
            {
                "id": "{{campaignId}}",
                "name": "{{name}}",
                "description": null,
                "emailTemplateId": "{{TemplateId}}",
                "status": "Open",
                "audienceGroupIds": [],
                "followUpEnabled": false,
                "followUpDueDays": 7,
                "followUpType": "Email",
                "createdAt": "2026-01-01T00:00:00Z",
                "updatedAt": "2026-01-01T00:00:00Z"
            }
            """;

        private static string BuildRecipients(Guid campaignId, int sentCount, int pendingCount)
        {
            var sent = Enumerable.Range(0, sentCount).Select(_ => RecipientJson(campaignId, "Sent"));
            var pending = Enumerable.Range(0, pendingCount).Select(_ => RecipientJson(campaignId, "Incorporated"));

            return $"[{string.Join(",", sent.Concat(pending))}]";
        }

        private static string RecipientJson(Guid campaignId, string status) => $$"""
            {
                "id": "{{Guid.NewGuid()}}",
                "campaignId": "{{campaignId}}",
                "contactId": "{{Guid.NewGuid()}}",
                "contactDisplayName": "Contact",
                "contactEmail": "contact@example.com",
                "messageTemplateId": "{{TemplateId}}",
                "status": "{{status}}",
                "emailDraftId": null,
                "exclusionReason": null,
                "incorporatedAt": "2026-01-01T00:00:00Z",
                "updatedAt": "2026-01-01T00:00:00Z"
            }
            """;

        private static string BuildCandidates(int count)
        {
            var items = Enumerable.Range(0, count).Select(index => $$"""
                {
                    "contactId": "{{Guid.NewGuid()}}",
                    "displayName": "Candidate {{index}}",
                    "email": "candidate{{index}}@example.com"
                }
                """);

            return $"[{string.Join(",", items)}]";
        }

        private static HttpResponseMessage JsonResponse(string json) =>
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
    }
}
