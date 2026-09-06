using System.Net;
using System.Text;
using System.Text.Json;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.ContactGroups;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class ContactGroupsComponentTests : BunitContext
{
    private static readonly Guid SchoolsGroupId = Guid.Parse("6a1b2c3d-4e5f-4a6b-8c7d-9e0f1a2b3c4d");
    private static readonly Guid UniversitiesGroupId = Guid.Parse("7b2c3d4e-5f60-4a7b-8c9d-0e1f2a3b4c5d");

    [Fact]
    public void ShouldShowMemberCountBadgePerGroup()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactGroups();

        component.WaitForAssertion(() => component.Markup.Should().Contain("Colegios de Asturias"));

        var rows = component.FindAll(".list-group-item");
        rows.First(row => row.TextContent.Contains("Colegios de Asturias")).TextContent.Should().Contain("42 contacts");
        rows.First(row => row.TextContent.Contains("Universidades")).TextContent.Should().Contain("7 contacts");
    }

    [Fact]
    public void ShouldFilterGroupsByName()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactGroups();

        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().HaveCount(2));

        component.Find("#contact-group-search").Input("Colegios");

        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().HaveCount(1));
        component.Markup.Should().Contain("Colegios de Asturias");
        component.Markup.Should().NotContain("Universidades");
    }

    [Fact]
    public void ShouldClearSearchFilter()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactGroups();

        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().HaveCount(2));

        component.Find("#contact-group-search").Input("Colegios");
        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().HaveCount(1));

        component.Find("button.btn-outline-secondary.w-100").Click();

        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().HaveCount(2));
    }

    [Fact]
    public void ShouldRequireAtLeastOneCriterionBeforeSubmitting()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContactGroups(out var handler);

        component.Find("#open-create-group-panel").Click();
        component.Find(".side-panel input.form-control").Change("No criteria group");
        component.Find("form").Submit();

        component.Markup.Should().Contain("Choose at least one criterion");
        handler.Requests.Should().NotContain(request => request.Method == HttpMethod.Post);
    }

    private IRenderedComponent<ContactGroups> RenderContactGroups() => RenderContactGroups(out _);

    private IRenderedComponent<ContactGroups> RenderContactGroups(out ContactGroupsHttpMessageHandler handler)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        handler = new ContactGroupsHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactGroupApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        return Render<ContactGroups>();
    }

    private sealed class ContactGroupsHttpMessageHandler : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            Requests.Add((request.Method, request.RequestUri?.PathAndQuery ?? string.Empty));

            if (request.Method == HttpMethod.Get && path == "/api/v1/contact-groups")
            {
                return Task.FromResult(JsonResponse(new object[]
                {
                    BuildGroup(SchoolsGroupId, "Colegios de Asturias"),
                    BuildGroup(UniversitiesGroupId, "Universidades")
                }));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/tags")
            {
                return Task.FromResult(JsonResponse(Array.Empty<object>()));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contact-groups/{SchoolsGroupId}/members")
            {
                return Task.FromResult(JsonResponse(BuildMembers(42)));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contact-groups/{UniversitiesGroupId}/members")
            {
                return Task.FromResult(JsonResponse(BuildMembers(7)));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }

        private static object BuildGroup(Guid id, string name) => new
        {
            id,
            name,
            createdAt = DateTimeOffset.UtcNow,
            updatedAt = DateTimeOffset.UtcNow,
            criteria = Array.Empty<object>()
        };

        private static object[] BuildMembers(int count) => Enumerable.Range(0, count)
            .Select(_ => (object)new { contactId = Guid.NewGuid(), isManualInclusion = false, isManualExclusion = false })
            .ToArray();

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
