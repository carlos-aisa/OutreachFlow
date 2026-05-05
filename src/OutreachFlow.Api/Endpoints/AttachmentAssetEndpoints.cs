using Microsoft.AspNetCore.Mvc;
using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;

namespace OutreachFlow.Api.Endpoints;

public static class AttachmentAssetEndpoints
{
    public static IEndpointRouteBuilder MapAttachmentAssetEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/attachments")
            .WithTags("AttachmentAssets");

        group.MapGet("", async (
            bool? activeOnly,
            IAttachmentAssetService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(activeOnly, cancellationToken))))
            .WithName("ListAttachmentAssets")
            .WithOpenApi();

        group.MapPost("", async (
            [FromForm] UploadAttachmentAssetForm request,
            IAttachmentAssetService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                if (request.File is null || request.File.Length <= 0)
                {
                    throw new ApplicationValidationException("Attachment file is required.");
                }

                await using var fileStream = request.File.OpenReadStream();
                var contentType = string.IsNullOrWhiteSpace(request.File.ContentType)
                    ? "application/octet-stream"
                    : request.File.ContentType;

                var attachment = await service.UploadAsync(
                    new UploadAttachmentAssetRequest(
                        request.Name,
                        request.Description,
                        request.File.FileName,
                        contentType,
                        fileStream,
                        request.File.Length),
                    cancellationToken);

                return Results.Created($"/api/v1/attachments/{attachment.Id}", attachment);
            }))
            .WithName("UploadAttachmentAsset")
            .DisableAntiforgery()
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IAttachmentAssetService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var attachment = await service.GetByIdAsync(id, cancellationToken);
                return attachment is null
                    ? ApiEndpoint.NotFound("Attachment asset was not found.")
                    : Results.Ok(attachment);
            }))
            .WithName("GetAttachmentAsset")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateAttachmentAssetRequest request,
            IAttachmentAssetService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateAttachmentAsset")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IAttachmentAssetService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeactivateAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeactivateAttachmentAsset")
            .WithOpenApi();

        return endpoints;
    }

    public sealed class UploadAttachmentAssetForm
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public IFormFile? File { get; set; }
    }
}
