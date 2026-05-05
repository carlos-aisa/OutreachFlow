using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Tags;

namespace OutreachFlow.Api.Endpoints;

public static class TagEndpoints
{
    public static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/tags")
            .WithTags("Tags");

        group.MapGet("", async (ITagService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(cancellationToken))))
            .WithName("ListTags")
            .WithOpenApi();

        group.MapPost("", async (
            CreateTagRequest request,
            ITagService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var tag = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/tags/{tag.Id}", tag);
            }))
            .WithName("CreateTag")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            ITagService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var tag = await service.GetByIdAsync(id, cancellationToken);
                return tag is null
                    ? ApiEndpoint.NotFound("Tag was not found.")
                    : Results.Ok(tag);
            }))
            .WithName("GetTag")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateTagRequest request,
            ITagService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateTag")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ITagService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeleteTag")
            .WithOpenApi();

        return endpoints;
    }
}
