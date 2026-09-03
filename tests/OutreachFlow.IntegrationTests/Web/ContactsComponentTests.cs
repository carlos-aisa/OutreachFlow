using System.Net;
using System.Text;
using System.Text.Json;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.Organizations;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class ContactsComponentTests : BunitContext
{
    [Fact]
    public void ShouldLinkToImportsPageNextToNewContactButton()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContacts(contactCount: 0);

        var importLink = component.Find("a[href='/imports']");
        importLink.TextContent.Trim().Should().Be("Import contacts");
        importLink.ClassList.Should().Contain("btn-outline-secondary");
    }

    [Fact]
    public void ShouldMarkFiltersCardAsOverflowVisibleSoDropdownsAreNotClipped()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContacts(contactCount: 0);

        var filtersCard = component.Find("#contact-organization-filter").Closest("section.card");
        filtersCard!.ClassList.Should().Contain("card--overflow-visible");
    }

    [Fact]
    public void ShouldPaginateTheContactsTableClientSide()
    {
        using var cultureScope = CultureTestScope.Use("en-US");
        using var component = RenderContacts(contactCount: 30);

        component.WaitForAssertion(() => component.FindAll("table tbody tr").Should().HaveCount(25));

        component.Markup.Should().Contain("1-25 of 30");

        component.Find(".pagination-controls button:last-child").Click();

        component.WaitForAssertion(() => component.FindAll("table tbody tr").Should().HaveCount(5));
    }

    private IRenderedComponent<Contacts> RenderContacts(int contactCount)
    {
        Services.AddLocalization(options => options.ResourcesPath = "Resources");

        var httpClient = new HttpClient(new ContactsHttpMessageHandler(contactCount))
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new OrganizationApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        return Render<Contacts>();
    }

    private sealed class ContactsHttpMessageHandler(int contactCount) : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;

            if (request.Method == HttpMethod.Get && path == "/api/v1/contacts")
            {
                return Task.FromResult(JsonResponse(BuildContacts(contactCount)));
            }

            return Task.FromResult(JsonResponse(Array.Empty<object>()));
        }

        private static object[] BuildContacts(int count)
        {
            var now = DateTimeOffset.UtcNow;
            return Enumerable.Range(0, count).Select(index => (object)new
            {
                id = Guid.NewGuid(),
                organizationId = (Guid?)null,
                organizationName = (string?)null,
                displayName = $"Contact {index}",
                email = $"contact{index}@example.com",
                phone = (string?)null,
                role = (string?)null,
                source = (string?)null,
                status = "New",
                doNotContact = false,
                lastContactedAt = (DateTimeOffset?)null,
                createdAt = now,
                updatedAt = now,
                tags = Array.Empty<object>()
            }).ToArray();
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
