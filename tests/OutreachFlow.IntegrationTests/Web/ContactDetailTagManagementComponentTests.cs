using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Web.Components.Pages;
using OutreachFlow.Web.Contacts;
using OutreachFlow.Web.FollowUps;
using OutreachFlow.Web.Tags;

namespace OutreachFlow.IntegrationTests.Web;

[Collection(CultureSensitiveTestCollectionDefinition.Name)]
public sealed class ContactDetailTagManagementComponentTests : BunitContext
{
    [Fact]
    public void ShouldEnableAssignTagButtonWhenAssignableTagsExist()
    {
        using var cultureScope = CultureTestScope.Use("en-US");

        var contactId = Guid.NewGuid();
        var candidateTag = new TagDto(Guid.NewGuid(), "Prospect", "Audience", DateTimeOffset.UtcNow);
        var handler = new ContactDetailHttpMessageHandler(
            contactId,
            [candidateTag],
            assignedTagIds: []);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new FollowUpTaskApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        var component = Render<ContactDetail>(parameters => parameters
            .Add(page => page.ContactId, contactId));

        component.WaitForAssertion(() =>
            component.Find("#assign-contact-tag").HasAttribute("disabled").Should().BeFalse());
    }

    [Fact]
    public void ShouldSendAssignTagRequestFromContactDetail()
    {
        using var cultureScope = CultureTestScope.Use("en-US");

        var contactId = Guid.NewGuid();
        var existingTag = new TagDto(Guid.NewGuid(), "VIP", "Audience", DateTimeOffset.UtcNow);
        var candidateTag = new TagDto(Guid.NewGuid(), "Prospect", "Audience", DateTimeOffset.UtcNow);
        var handler = new ContactDetailHttpMessageHandler(
            contactId,
            [existingTag, candidateTag],
            [existingTag.Id]);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        Services.AddLocalization(options => options.ResourcesPath = "Resources");
        Services.AddSingleton(new ContactApiClient(httpClient));
        Services.AddSingleton(new FollowUpTaskApiClient(httpClient));
        Services.AddSingleton(new TagApiClient(httpClient));

        var component = Render<ContactDetail>(parameters => parameters
            .Add(page => page.ContactId, contactId));

        component.WaitForAssertion(() =>
            component.FindAll("#contact-tag-select option").Should().HaveCount(2));

        component.Find("#contact-tag-select").Change(candidateTag.Id.ToString());
        component.Find("#assign-contact-tag").Click();

        component.WaitForAssertion(() =>
            handler.Requests.Should().Contain(request =>
                request.Method == HttpMethod.Post &&
                request.PathAndQuery == $"/api/v1/contacts/{contactId}/tags/{candidateTag.Id}"));
    }

    private sealed class ContactDetailHttpMessageHandler(
        Guid contactId,
        IReadOnlyList<TagDto> availableTags,
        IEnumerable<Guid> assignedTagIds) : HttpMessageHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();
        private readonly HashSet<Guid> assigned = [..assignedTagIds];

        public List<(HttpMethod Method, string PathAndQuery)> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;
            var pathAndQuery = request.RequestUri?.PathAndQuery ?? string.Empty;
            Requests.Add((request.Method, pathAndQuery));

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contacts/{contactId}")
            {
                return Task.FromResult(JsonResponse(BuildContact(contactId, availableTags, assigned)));
            }

            if (request.Method == HttpMethod.Get && path == $"/api/v1/contacts/{contactId}/activities")
            {
                return Task.FromResult(JsonResponse(Array.Empty<object>()));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/follow-ups")
            {
                return Task.FromResult(JsonResponse(Array.Empty<object>()));
            }

            if (request.Method == HttpMethod.Get && path == "/api/v1/tags")
            {
                return Task.FromResult(JsonResponse(availableTags));
            }

            if (request.Method == HttpMethod.Post &&
                path.StartsWith($"/api/v1/contacts/{contactId}/tags/", StringComparison.Ordinal))
            {
                var tagId = ParseTagId(path);
                assigned.Add(tagId);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            }

            if (request.Method == HttpMethod.Delete &&
                path.StartsWith($"/api/v1/contacts/{contactId}/tags/", StringComparison.Ordinal))
            {
                var tagId = ParseTagId(path);
                assigned.Remove(tagId);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }

        private static ContactDto BuildContact(
            Guid contactId,
            IReadOnlyList<TagDto> availableTags,
            HashSet<Guid> assignedTagIds)
        {
            var contactTags = availableTags
                .Where(tag => assignedTagIds.Contains(tag.Id))
                .Select(tag => new ContactTagDto(tag.Id, tag.Name, tag.Category))
                .ToArray();

            var now = DateTimeOffset.UtcNow;

            return new ContactDto(
                contactId,
                OrganizationId: null,
                OrganizationName: null,
                DisplayName: "Alice Example",
                Email: "alice@example.com",
                Phone: null,
                Role: null,
                Source: null,
                Status: ContactStatus.New,
                DoNotContact: false,
                LastContactedAt: null,
                CreatedAt: now,
                UpdatedAt: now,
                Tags: contactTags);
        }

        private static Guid ParseTagId(string path)
        {
            var value = path.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
            return Guid.Parse(value);
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
    }
}
