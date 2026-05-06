using OutreachFlow.Api.Errors;
using OutreachFlow.Application.FollowUps;

namespace OutreachFlow.Api.Endpoints;

public static class FollowUpTaskEndpoints
{
    public static IEndpointRouteBuilder MapFollowUpTaskEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/follow-ups")
            .WithTags("FollowUps");

        group.MapGet("", async (
            Guid? contactId,
            bool? isCompleted,
            DateTimeOffset? dueFrom,
            DateTimeOffset? dueTo,
            int? limit,
            IFollowUpTaskService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(
                    new FollowUpTaskFilterRequest(contactId, isCompleted, dueFrom, dueTo, limit),
                    cancellationToken))))
            .WithName("ListFollowUpTasks")
            .WithOpenApi();

        group.MapPost("", async (
            CreateFollowUpTaskRequest request,
            IFollowUpTaskService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var task = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/follow-ups/{task.Id}", task);
            }))
            .WithName("CreateFollowUpTask")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IFollowUpTaskService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var task = await service.GetByIdAsync(id, cancellationToken);
                return task is null
                    ? ApiEndpoint.NotFound("Follow-up task was not found.")
                    : Results.Ok(task);
            }))
            .WithName("GetFollowUpTask")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateFollowUpTaskRequest request,
            IFollowUpTaskService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateFollowUpTask")
            .WithOpenApi();

        group.MapPost("/{id:guid}/complete", async (
            Guid id,
            IFollowUpTaskService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.CompleteAsync(id, cancellationToken))))
            .WithName("CompleteFollowUpTask")
            .WithOpenApi();

        return endpoints;
    }
}
