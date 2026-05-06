using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.ContactImports;
using OutreachFlow.Application.Tags;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class ContactImportEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldPreviewCsvImportWithValidationAndDuplicates()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        _ = await PostAsync<TagDto>(
            client,
            "/api/v1/tags",
            new CreateTagRequest("Lead", null));
        _ = await PostAsync<ContactImportCommitResult>(
            client,
            "/api/v1/contact-imports/commit",
            new ContactImportCommitRequest(
                "seed.csv",
                """
                displayName,email
                Existing Contact,existing@example.com
                """,
                []));

        var preview = await PostAsync<ContactImportPreviewResult>(
            client,
            "/api/v1/contact-imports/preview",
            new ContactImportPreviewRequest(
                "contacts.csv",
                """
                displayName,email
                Existing Contact,existing@example.com
                Alex Morgan,alex@example.com
                Alex Duplicate,alex@example.com
                ,missing@example.com
                """));

        preview.TotalRows.Should().Be(4);
        preview.ValidRows.Should().Be(1);
        preview.DuplicateRows.Should().Be(2);
        preview.InvalidRows.Should().Be(1);
    }

    [Fact]
    public async Task ShouldCommitCsvImportAssignTagsAndStoreImportJob()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        var tag = await PostAsync<TagDto>(
            client,
            "/api/v1/tags",
            new CreateTagRequest("Imported", "Source"));

        var commit = await PostAsync<ContactImportCommitResult>(
            client,
            "/api/v1/contact-imports/commit",
            new ContactImportCommitRequest(
                "contacts.csv",
                """
                displayName,email,phone,role,source
                Alex Morgan,alex@example.com,+34 600 000 000,Founder,CSV
                Sam Taylor,sam@example.com,,Operations,CSV
                Sam Duplicate,sam@example.com,,Operations,CSV
                """,
                [tag.Id]));

        commit.TotalRows.Should().Be(3);
        commit.CreatedCount.Should().Be(2);
        commit.DuplicateCount.Should().Be(1);
        commit.InvalidCount.Should().Be(0);

        var contacts = await GetAsync<JsonElement>(client, "/api/v1/contacts");
        contacts.GetArrayLength().Should().Be(2);
        foreach (var contact in contacts.EnumerateArray())
        {
            var tags = contact.GetProperty("tags");
            tags.GetArrayLength().Should().Be(1);
            tags[0].GetProperty("id").GetGuid().Should().Be(tag.Id);
        }

        var jobs = await GetAsync<IReadOnlyList<ImportJobDto>>(
            client,
            "/api/v1/contact-imports/jobs?limit=5");
        jobs.Should().ContainSingle(job =>
            job.Id == commit.ImportJobId &&
            job.Status.ToString() == "Completed" &&
            job.CreatedCount == 2 &&
            job.DuplicateRows == 1);
    }

    [Fact]
    public async Task ShouldReturnValidationErrorWhenRequiredHeaderIsMissing()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/api/v1/contact-imports/preview",
            new ContactImportPreviewRequest(
                "contacts.csv",
                """
                name,email
                Alex Morgan,alex@example.com
                """),
            JsonOptions);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static async Task<T> GetAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> PostAsync<T>(HttpClient client, string uri, object request)
    {
        using var response = await client.PostAsJsonAsync(uri, request, JsonOptions);
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
