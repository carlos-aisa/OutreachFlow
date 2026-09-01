using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.ContactGroups;
using OutreachFlow.Web.FollowUps;
using OutreachFlow.Web.Organizations;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class ContactDetailComponentTests : BunitContext
{
    private static readonly Guid ContactId = Guid.Parse("1a2b3c4d-5e6f-4a7b-8c9d-0e1f2a3b4c5d");
    private static readonly Guid ExistingTagId = Guid.Parse("2b3c4d5e-6f7a-4b8c-9d0e-1f2a3b4c5d6e");
    private static readonly Guid CandidateTagId = Guid.Parse("3c4d5e6f-7a8b-4c9d-0e1f-2a3b4c5d6e7f");
    private static readonly Guid ExistingGroupId = Guid.Parse("4d5e6f7a-8b9c-4d0e-1f2a-3b4c5d6e7f8a");
    private static readonly Guid CandidateGroupId = Guid.Parse("5e6f7a8b-9c0d-4e1f-2a3b-4c5d6e7f8a9b");

    [Fact]
    public void ShouldShowGroupsAndTagsReadOnlyOnTheSummaryCard()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out _);

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("VIP");
            component.Markup.Should().Contain("Prospects");
        });

        component.FindAll("#contact-add-tag").Should().BeEmpty();
        component.FindAll("#contact-add-group").Should().BeEmpty();
        component.FindAll(".chip-remove").Should().BeEmpty();
    }

    [Fact]
    public void ShouldOnlyOfferUnassignedTagsInEditPanelAddDropdown()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out _);

        OpenEditPanel(component);
        component.Find("#contact-edit-add-tag").Click();

        var options = component.FindAll(".combo-item").Select(item => item.TextContent.Trim()).ToArray();
        options.Should().Contain("Audience: Prospect");
        options.Should().NotContain("Audience: VIP");
    }

    [Fact]
    public void ShouldSendAssignTagRequestFromEditPanelChipPicker()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out var handler);

        OpenEditPanel(component);
        component.Find("#contact-edit-add-tag").Click();
        component.Find("#contact-edit-add-tag-search").Input("Prospect");
        component.Find(".combo-item").Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Post &&
                request.PathAndQuery == $"/api/v1/contacts/{ContactId}/tags/{CandidateTagId}"));
    }

    [Fact]
    public void ShouldSendRemoveTagRequestFromEditPanelChipPicker()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out var handler);

        OpenEditPanel(component);
        component.WaitForAssertion(() => component.Find(".side-panel").TextContent.Should().Contain("VIP"));
        var tagChip = component.Find(".side-panel").QuerySelectorAll(".chip").First(chip => chip.TextContent.Contains("VIP"));
        tagChip.QuerySelector(".chip-remove")!.Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Delete &&
                request.PathAndQuery == $"/api/v1/contacts/{ContactId}/tags/{ExistingTagId}"));
    }

    [Fact]
    public void ShouldSendIncludeOverrideWhenAddingGroupChipFromEditPanel()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out var handler);

        OpenEditPanel(component);
        component.Find("#contact-edit-add-group").Click();
        component.Find("#contact-edit-add-group-search").Input("Leads");
        component.Find(".combo-item").Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery.Contains($"/api/v1/contact-groups/{CandidateGroupId}/members/{ContactId}/membership-override") &&
                request.PathAndQuery.Contains("type=Include")));
    }

    [Fact]
    public void ShouldSendExcludeOverrideWhenRemovingGroupChipFromEditPanel()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out var handler);

        OpenEditPanel(component);
        component.WaitForAssertion(() => component.Find(".side-panel").TextContent.Should().Contain("Prospects"));
        var groupChip = component.Find(".side-panel").QuerySelectorAll(".chip").First(chip => chip.TextContent.Contains("Prospects"));
        groupChip.QuerySelector(".chip-remove")!.Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery.Contains($"/api/v1/contact-groups/{ExistingGroupId}/members/{ContactId}/membership-override") &&
                request.PathAndQuery.Contains("type=Exclude")));
    }

    [Fact]
    public void ShouldSendUpdateContactRequestFromEditPanel()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactDetail(out var handler);

        OpenEditPanel(component);
        component.Find("#contact-edit-name").Change("Alice Updated");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery == $"/api/v1/contacts/{ContactId}"));
        component.Markup.Should().Contain("Alice Updated");
    }

    private static void OpenEditPanel(IRenderedComponent<ContactDetail> component)
    {
        component.WaitForAssertion(() => component.Markup.Should().Contain("Edit"));
        component.FindAll("button").First(button => button.TextContent.Trim() == "Edit").Click();
    }

    private IRenderedComponent<ContactDetail> RenderContactDetail(out ContactDetailHttpMessageHandler handler)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        handler = new ContactDetailHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new ContactGroupApiClient(httpClient));
        Services.AddSingleton(new FollowUpTaskApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        return Render<ContactDetail>(parameters => parameters.Add(page => page.ContactId, ContactId));
    }

    private sealed class ContactDetailHttpMessageHandler : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

        private readonly HashSet<Guid> assignedTagIds = [ExistingTagId];
        private readonly HashSet<Guid> memberGroupIds = [ExistingGroupId];
        private string displayName = "Alice Example";
        private string email = "alice@example.com";

        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var pathAndQuery = request.RequestUri?.PathAndQuery ?? string.Empty;
            Requests.Add((request.Method, pathAndQuery));

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contacts/{ContactId}")
            {
                return JsonResponse(BuildContact());
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contacts/{ContactId}/activities")
            {
                return JsonResponse(Array.Empty<object>());
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/follow-ups")
            {
                return JsonResponse(Array.Empty<object>());
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/tags")
            {
                return JsonResponse(new object[]
                {
                    new { id = ExistingTagId, name = "VIP", category = "Audience", createdAt = DateTimeOffset.UtcNow },
                    new { id = CandidateTagId, name = "Prospect", category = "Audience", createdAt = DateTimeOffset.UtcNow }
                });
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/contact-groups")
            {
                return JsonResponse(new object[]
                {
                    BuildGroup(ExistingGroupId, "Prospects"),
                    BuildGroup(CandidateGroupId, "Leads")
                });
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contacts/{ContactId}/groups")
            {
                var groups = new List<object>();
                if (memberGroupIds.Contains(ExistingGroupId)) groups.Add(BuildGroup(ExistingGroupId, "Prospects"));
                if (memberGroupIds.Contains(CandidateGroupId)) groups.Add(BuildGroup(CandidateGroupId, "Leads"));
                return JsonResponse(groups);
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/organizations")
            {
                return JsonResponse(Array.Empty<object>());
            }

            if (request.Method == HttpMethod.Post &&
                path.StartsWith($"/api/v1/contacts/{ContactId}/tags/", StringComparison.Ordinal))
            {
                assignedTagIds.Add(ParseLastSegment(path));
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            if (request.Method == HttpMethod.Delete &&
                path.StartsWith($"/api/v1/contacts/{ContactId}/tags/", StringComparison.Ordinal))
            {
                assignedTagIds.Remove(ParseLastSegment(path));
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            if (request.Method == HttpMethod.Put &&
                path.Contains("/membership-override", StringComparison.Ordinal))
            {
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var groupId = Guid.Parse(segments[3]);
                var type = HttpUtility.ParseQueryString(request.RequestUri!.Query)["type"];

                if (string.Equals(type, "Include", StringComparison.OrdinalIgnoreCase))
                {
                    memberGroupIds.Add(groupId);
                }
                else
                {
                    memberGroupIds.Remove(groupId);
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            if (request.Method == HttpMethod.Put && path == $"/api/v1/contacts/{ContactId}")
            {
                var body = await request.Content!.ReadFromJsonAsync<UpdateContactBody>(JsonOptions, cancellationToken);
                displayName = body!.DisplayName;
                email = body.Email;
                return JsonResponse(BuildContact());
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }

        private object BuildContact()
        {
            var tags = new List<object>();
            if (assignedTagIds.Contains(ExistingTagId)) tags.Add(new { id = ExistingTagId, name = "VIP", category = "Audience" });
            if (assignedTagIds.Contains(CandidateTagId)) tags.Add(new { id = CandidateTagId, name = "Prospect", category = "Audience" });

            var now = DateTimeOffset.UtcNow;

            return new
            {
                id = ContactId,
                organizationId = (Guid?)null,
                organizationName = (string?)null,
                displayName,
                email,
                phone = (string?)null,
                role = (string?)null,
                source = (string?)null,
                status = "New",
                doNotContact = false,
                lastContactedAt = (DateTimeOffset?)null,
                createdAt = now,
                updatedAt = now,
                tags
            };
        }

        private static object BuildGroup(Guid id, string name) => new
        {
            id,
            name,
            createdAt = DateTimeOffset.UtcNow,
            updatedAt = DateTimeOffset.UtcNow,
            criteria = Array.Empty<object>()
        };

        private static Guid ParseLastSegment(string path)
        {
            return Guid.Parse(path.Split('/', StringSplitOptions.RemoveEmptyEntries).Last());
        }

        private static HttpResponseMessage JsonResponse<T>(T payload)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload, JsonOptions),
                    Encoding.UTF8,
                    "application/json")
            };
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        private sealed record UpdateContactBody(
            Guid? OrganizationId,
            string DisplayName,
            string Email,
            string? Phone,
            string? Role,
            string? Source,
            string Status,
            bool DoNotContact);
    }
}
