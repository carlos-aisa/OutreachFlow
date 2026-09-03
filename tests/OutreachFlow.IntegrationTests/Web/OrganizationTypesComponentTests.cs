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
public sealed class OrganizationTypesComponentTests : BunitContext
{
    private static readonly Guid OrganizationTypeId = Guid.Parse("3d4e5f60-7182-4c9d-9e0f-1a2b3c4d5e6f");

    [Fact]
    public void ShouldCreateOrganizationTypeAndSendCreateRequest()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderOrganizationTypes(out var handler, seedExisting: false);

        component.Find("#open-create-organization-type-panel").Click();
        component.Find("#organization-type-name").Change("Colegio");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Post &&
                request.PathAndQuery == "/api/v1/organization-types"));

        component.Markup.Should().Contain("Colegio");
    }

    [Fact]
    public void ShouldEditOrganizationTypeAndSendUpdateRequest()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderOrganizationTypes(out var handler, seedExisting: true);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Colegio"));

        component.Find("button.btn-outline-primary").Click();
        component.Find("#organization-type-name").Change("Universidad");
        component.Find("form").Submit();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Put &&
                request.PathAndQuery == $"/api/v1/organization-types/{OrganizationTypeId}"));

        component.Markup.Should().Contain("Universidad");
    }

    [Fact]
    public void ShouldDeleteOrganizationTypeAfterConfirmation()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderOrganizationTypes(out var handler, seedExisting: true);

        JSInterop.Setup<bool>("confirm", _ => true).SetResult(true);

        component.WaitForAssertion(() => component.Markup.Should().Contain("Colegio"));

        component.Find("button.btn-outline-danger").Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Delete &&
                request.PathAndQuery == $"/api/v1/organization-types/{OrganizationTypeId}"));

        component.Markup.Should().Contain("No organization types have been created yet.");
    }

    private IRenderedComponent<OrganizationTypes> RenderOrganizationTypes(
        out OrganizationTypesHttpMessageHandler handler,
        bool seedExisting)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        handler = new OrganizationTypesHttpMessageHandler(seedExisting);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new OrganizationTypeApiClient(httpClient));

        return Render<OrganizationTypes>();
    }

    private sealed class OrganizationTypesHttpMessageHandler(bool seedExisting) : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private string name = "Colegio";
        private bool created = seedExisting;
        private bool deleted;

        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var pathAndQuery = request.RequestUri?.PathAndQuery ?? string.Empty;
            Requests.Add((request.Method, pathAndQuery));

            if (request.Method == HttpMethod.Get && path == "/api/v1/organization-types")
            {
                return JsonResponse(deleted || !created ? Array.Empty<object>() : [BuildOrganizationType()]);
            }

            if (request.Method == HttpMethod.Post && path == "/api/v1/organization-types")
            {
                var body = await request.Content!.ReadFromJsonAsync<OrganizationTypeBody>(JsonOptions, cancellationToken);
                name = body!.Name;
                created = true;
                return JsonResponse(BuildOrganizationType());
            }

            if (request.Method == HttpMethod.Put && path == $"/api/v1/organization-types/{OrganizationTypeId}")
            {
                var body = await request.Content!.ReadFromJsonAsync<OrganizationTypeBody>(JsonOptions, cancellationToken);
                name = body!.Name;
                return JsonResponse(BuildOrganizationType());
            }

            if (request.Method == HttpMethod.Delete && path == $"/api/v1/organization-types/{OrganizationTypeId}")
            {
                deleted = true;
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
        }

        private object BuildOrganizationType()
        {
            return new
            {
                id = OrganizationTypeId,
                name,
                createdAt = DateTimeOffset.UtcNow
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

        private sealed record OrganizationTypeBody(string Name);
    }
}
