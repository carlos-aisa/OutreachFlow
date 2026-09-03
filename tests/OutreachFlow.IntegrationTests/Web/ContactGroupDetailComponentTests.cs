using System.Net;
using System.Text;
using System.Text.Json;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.ContactGroups;
using OutreachFlow.Web.Contacts;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class ContactGroupDetailComponentTests : BunitContext
{
    private static readonly Guid GroupId = Guid.Parse("5e6f7a8b-9c0d-4e1f-2a3b-4c5d6e7f8a9b");
    private static readonly Guid AnaId = Guid.Parse("1a1a1a1a-1a1a-1a1a-1a1a-1a1a1a1a1a1a");
    private static readonly Guid LuisId = Guid.Parse("2b2b2b2b-2b2b-2b2b-2b2b-2b2b2b2b2b2b");
    private static readonly Guid MartaId = Guid.Parse("3c3c3c3c-3c3c-3c3c-3c3c-3c3c3c3c3c3c");
    private static readonly Guid JorgeId = Guid.Parse("4d4d4d4d-4d4d-4d4d-4d4d-4d4d4d4d4d4d");

    [Fact]
    public void ShouldShowMembershipStatusForEachContact()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderDetail(out _);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Ana Pérez"));

        var rows = component.FindAll("table tbody tr");
        rows.Should().HaveCount(4);
        component.Markup.Should().Contain("Member (by criteria)");
        component.Markup.Should().Contain("Not a member");
        component.Markup.Should().Contain("Excluded manually");
        component.Markup.Should().Contain("Added manually");
    }

    [Fact]
    public void ShouldIncludeANonMemberContact()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderDetail(out var handler);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Luis Gómez"));

        var row = component.FindAll("table tbody tr").First(r => r.TextContent.Contains("Luis Gómez"));
        row.QuerySelector("button")!.Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery.Contains($"/api/v1/contact-groups/{GroupId}/members/{LuisId}/membership-override") &&
                request.PathAndQuery.Contains("type=Include")));
    }

    [Fact]
    public void ShouldExcludeAMemberByCriteria()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderDetail(out var handler);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Ana Pérez"));

        var row = component.FindAll("table tbody tr").First(r => r.TextContent.Contains("Ana Pérez"));
        row.QuerySelector("button")!.Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery.Contains($"/api/v1/contact-groups/{GroupId}/members/{AnaId}/membership-override") &&
                request.PathAndQuery.Contains("type=Exclude")));
    }

    [Fact]
    public void ShouldClearOverrideAndRefreshStatus()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderDetail(out var handler);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Jorge Díaz"));

        var row = component.FindAll("table tbody tr").First(r => r.TextContent.Contains("Jorge Díaz"));
        row.QuerySelector("button")!.Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Delete &&
                request.PathAndQuery == $"/api/v1/contact-groups/{GroupId}/members/{JorgeId}/membership-override"));

        component.WaitForAssertion(() =>
            component.FindAll("table tbody tr").First(r => r.TextContent.Contains("Jorge Díaz")).TextContent.Should().Contain("Not a member"));
    }

    [Fact]
    public void ShouldFilterContactsBySearch()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderDetail(out _);

        component.WaitForAssertion(() => component.FindAll("table tbody tr").Should().HaveCount(4));

        component.Find("#contact-group-member-search").Input("Ana");

        component.WaitForAssertion(() => component.FindAll("table tbody tr").Should().HaveCount(1));
        component.Markup.Should().Contain("Ana Pérez");
    }

    private IRenderedComponent<ContactGroupDetail> RenderDetail(out ContactGroupDetailHttpMessageHandler handler)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        handler = new ContactGroupDetailHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactGroupApiClient(httpClient));
        Services.AddSingleton(new ContactApiClient(httpClient));

        return Render<ContactGroupDetail>(parameters => parameters.Add(page => page.Id, GroupId));
    }

    private sealed class ContactGroupDetailHttpMessageHandler : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly Dictionary<Guid, string> overrides = new()
        {
            [MartaId] = "Exclude",
            [JorgeId] = "Include"
        };

        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var pathAndQuery = request.RequestUri?.PathAndQuery ?? string.Empty;
            Requests.Add((request.Method, pathAndQuery));

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contact-groups/{GroupId}")
            {
                return Task.FromResult(JsonResponse(new
                {
                    id = GroupId,
                    name = "Asturias",
                    createdAt = DateTimeOffset.UtcNow,
                    updatedAt = DateTimeOffset.UtcNow,
                    criteria = new object[] { new { type = "Province", value = "Asturias" } }
                }));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/contacts")
            {
                return Task.FromResult(JsonResponse(new object[]
                {
                    BuildContact(AnaId, "Ana Pérez", "ana@example.com", "Asturias"),
                    BuildContact(LuisId, "Luis Gómez", "luis@example.com", null),
                    BuildContact(MartaId, "Marta Ruiz", "marta@example.com", "Asturias"),
                    BuildContact(JorgeId, "Jorge Díaz", "jorge@example.com", null)
                }));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contact-groups/{GroupId}/membership-status")
            {
                return Task.FromResult(JsonResponse(new object[]
                {
                    BuildStatus(AnaId, "Asturias"),
                    BuildStatus(LuisId, null),
                    BuildStatus(MartaId, "Asturias"),
                    BuildStatus(JorgeId, null)
                }));
            }

            if (request.Method == HttpMethod.Put && path.Contains("/membership-override", StringComparison.Ordinal))
            {
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var contactId = Guid.Parse(segments[5]);
                var type = System.Web.HttpUtility.ParseQueryString(request.RequestUri!.Query)["type"]!;
                overrides[contactId] = type;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            }

            if (request.Method == HttpMethod.Delete && path.Contains("/membership-override", StringComparison.Ordinal))
            {
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var contactId = Guid.Parse(segments[5]);
                overrides.Remove(contactId);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }

        private object BuildStatus(Guid contactId, string? province)
        {
            var status = overrides.TryGetValue(contactId, out var overrideType)
                ? overrideType == "Include" ? "MemberByManualInclusion" : "ExcludedManually"
                : province == "Asturias" ? "MemberByCriteria" : "NotAMember";

            return new { contactId, status };
        }

        private static object BuildContact(Guid id, string displayName, string email, string? province)
        {
            var now = DateTimeOffset.UtcNow;
            return new
            {
                id,
                organizationId = (Guid?)null,
                organizationName = province is null ? null : $"Org in {province}",
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
                tags = Array.Empty<object>()
            };
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
    }
}
