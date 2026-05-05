using OutreachFlow.Api.Errors;
using OutreachFlow.Application.SenderProfiles;

namespace OutreachFlow.Api.Endpoints;

public static class SenderProfileEndpoints
{
    public static IEndpointRouteBuilder MapSenderProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/sender-profiles")
            .WithTags("SenderProfiles");

        group.MapGet("", async (
            bool? activeOnly,
            ISenderProfileService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(activeOnly, cancellationToken))))
            .WithName("ListSenderProfiles")
            .WithOpenApi();

        group.MapPost("", async (
            CreateSenderProfileRequest request,
            ISenderProfileService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var senderProfile = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/sender-profiles/{senderProfile.Id}", senderProfile);
            }))
            .WithName("CreateSenderProfile")
            .WithOpenApi();

        group.MapGet("/default", async (
            ISenderProfileService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var senderProfile = await service.GetDefaultAsync(cancellationToken);
                return senderProfile is null
                    ? ApiEndpoint.NotFound("Default sender profile was not found.")
                    : Results.Ok(senderProfile);
            }))
            .WithName("GetDefaultSenderProfile")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISenderProfileService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var senderProfile = await service.GetByIdAsync(id, cancellationToken);
                return senderProfile is null
                    ? ApiEndpoint.NotFound("Sender profile was not found.")
                    : Results.Ok(senderProfile);
            }))
            .WithName("GetSenderProfile")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateSenderProfileRequest request,
            ISenderProfileService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateSenderProfile")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ISenderProfileService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeactivateAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeactivateSenderProfile")
            .WithOpenApi();

        return endpoints;
    }
}
