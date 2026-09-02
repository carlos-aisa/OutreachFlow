using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.Organizations;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class OrganizationsComponentTests : BunitContext
{
    private static readonly Guid OrganizationId = Guid.Parse("1b2c3d4e-5f60-4a7b-8c9d-0e1f2a3b4c5d");

    [Fact]
    public void ShouldEditOrganizationAndSendUpdateRequest()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderOrganizations(out var handler);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Acme Corp"));

        component.Find("button.btn-outline-primary").Click();

        component.Markup.Should().Contain("Edit organization");
        component.Find("#organization-name").GetAttribute("value").Should().Be("Acme Corp");

        component.Find("#organization-name").Change("Acme Corporation");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery == $"/api/v1/organizations/{OrganizationId}"));

        component.Markup.Should().Contain("Acme Corporation");
    }

    [Fact]
    public void ShouldDeleteOrganizationAfterConfirmationAndSendDeleteRequest()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderOrganizations(out var handler);

        JSInterop.Setup<bool>("confirm", invocation =>
            invocation.Arguments.Count == 1 &&
            invocation.Arguments[0]!.ToString()!.Contains("Acme Corp", StringComparison.Ordinal))
            .SetResult(true);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Acme Corp"));

        component.Find("button.btn-outline-danger").Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Delete &&
                request.PathAndQuery == $"/api/v1/organizations/{OrganizationId}"));

        component.Markup.Should().NotContain("Acme Corp");
    }

    [Fact]
    public void ShouldNotDeleteOrganizationWhenConfirmationIsDeclined()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderOrganizations(out var handler);

        JSInterop.Setup<bool>("confirm", _ => true).SetResult(false);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Acme Corp"));

        component.Find("button.btn-outline-danger").Click();

        handler.Requests.Should().NotContain(request => request.Method == HttpMethod.Delete);
        component.Markup.Should().Contain("Acme Corp");
    }

    private IRenderedComponent<Organizations> RenderOrganizations(out OrganizationsHttpMessageHandler handler)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        handler = new OrganizationsHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new OrganizationApiClient(httpClient));

        return Render<Organizations>();
    }

    private sealed class OrganizationsHttpMessageHandler : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private string name = "Acme Corp";
        private bool deleted;

        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var pathAndQuery = request.RequestUri?.PathAndQuery ?? string.Empty;
            Requests.Add((request.Method, pathAndQuery));

            if (request.Method == HttpMethod.Get && path == "/api/v1/organizations")
            {
                return JsonResponse(deleted ? Array.Empty<object>() : [BuildOrganization()]);
            }

            if (request.Method == HttpMethod.Put && path == $"/api/v1/organizations/{OrganizationId}")
            {
                var body = await request.Content!.ReadFromJsonAsync<UpdateOrganizationBody>(JsonOptions, cancellationToken);
                name = body!.Name;
                return JsonResponse(BuildOrganization());
            }

            if (request.Method == HttpMethod.Delete && path == $"/api/v1/organizations/{OrganizationId}")
            {
                deleted = true;
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }

        private object BuildOrganization()
        {
            var now = DateTimeOffset.UtcNow;
            return new
            {
                id = OrganizationId,
                name,
                type = (string?)null,
                website = (string?)null,
                city = (string?)null,
                province = (string?)null,
                country = (string?)null,
                notes = (string?)null,
                createdAt = now,
                updatedAt = now
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

        private sealed record UpdateOrganizationBody(
            string Name,
            string? Type,
            string? Website,
            string? City,
            string? Province,
            string? Country,
            string? Notes);
    }
}
