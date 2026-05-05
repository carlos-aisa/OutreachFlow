using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.EmailTemplates;

namespace OutreachFlow.IntegrationTests.Api;

public sealed class AttachmentAssetsEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    [Fact]
    public async Task ShouldUploadListAndDeactivateAttachmentAsset()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var uploadedAttachment = await UploadAttachmentAsync(
            client,
            "Service brochure",
            "brochure.pdf",
            "application/pdf",
            [1, 2, 3, 4]);

        var attachments = await GetAsync<IReadOnlyList<AttachmentAssetDto>>(client, "/api/v1/attachments");
        attachments.Should().ContainSingle(item => item.Id == uploadedAttachment.Id);

        var deactivateResponse = await client.DeleteAsync($"/api/v1/attachments/{uploadedAttachment.Id}");
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var activeAttachments = await GetAsync<IReadOnlyList<AttachmentAssetDto>>(
            client,
            "/api/v1/attachments?activeOnly=true");
        activeAttachments.Should().NotContain(item => item.Id == uploadedAttachment.Id);
    }

    [Fact]
    public async Task ShouldAssignAndRemoveDefaultAttachmentFromTemplate()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        var uploadedAttachment = await UploadAttachmentAsync(
            client,
            "Service brochure",
            "brochure.pdf",
            "application/pdf",
            [1, 2, 3, 4]);
        var template = await PostAsync<EmailTemplateDto>(
            client,
            "/api/v1/templates",
            new CreateEmailTemplateRequest(
                "Intro",
                null,
                "Subject",
                "Body"));

        var templateWithAttachment = await PostWithoutBodyAsync<EmailTemplateDto>(
            client,
            $"/api/v1/templates/{template.Id}/attachments/{uploadedAttachment.Id}");
        templateWithAttachment.DefaultAttachmentIds.Should().Contain(uploadedAttachment.Id);

        var templateWithoutAttachment = await DeleteAndReadAsync<EmailTemplateDto>(
            client,
            $"/api/v1/templates/{template.Id}/attachments/{uploadedAttachment.Id}");
        templateWithoutAttachment.DefaultAttachmentIds.Should().NotContain(uploadedAttachment.Id);
    }

    [Fact]
    public async Task ShouldRejectUnsafeAttachmentPathInput()
    {
        using var factory = new OutreachFlowApiFactory();
        await factory.InitializeDatabaseAsync();
        using var client = factory.CreateClient();
        using var formData = new MultipartFormDataContent
        {
            { new StringContent("Unsafe brochure"), "name" }
        };
        using var fileContent = new ByteArrayContent([1, 2, 3]);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
        formData.Add(fileContent, "file", "../unsafe.pdf");

        using var response = await client.PostAsync("/api/v1/attachments", formData);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.ToLowerInvariant().Should().Contain("unsafe path");
    }

    private static async Task<AttachmentAssetDto> UploadAttachmentAsync(
        HttpClient client,
        string name,
        string fileName,
        string contentType,
        byte[] payload)
    {
        using var formData = new MultipartFormDataContent
        {
            { new StringContent(name), "name" }
        };

        using var fileContent = new ByteArrayContent(payload);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        formData.Add(fileContent, "file", fileName);

        using var response = await client.PostAsync("/api/v1/attachments", formData);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<AttachmentAssetDto>(response);
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

    private static async Task<T> PostWithoutBodyAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.PostAsync(uri, null);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> DeleteAndReadAsync<T>(HttpClient client, string uri)
    {
        using var response = await client.DeleteAsync(uri);
        response.EnsureSuccessStatusCode();
        return await ReadAsync<T>(response);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            throw new InvalidOperationException("The API returned an empty response.");
        }

        var result = JsonSerializer.Deserialize<T>(responseBody, JsonOptions);
        return result ?? throw new InvalidOperationException("The API returned an empty response.");
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
