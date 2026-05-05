using OutreachFlow.Api.Errors;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Domain.EmailDrafts;

namespace OutreachFlow.Api.Endpoints;

public static class EmailDraftEndpoints
{
    public static IEndpointRouteBuilder MapEmailDraftEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/drafts")
            .WithTags("EmailDrafts");

        group.MapPost("/generate", async (
            GenerateEmailDraftsRequest request,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.GenerateAsync(request, cancellationToken))))
            .WithName("GenerateEmailDrafts")
            .WithOpenApi();

        group.MapGet("", async (
            EmailDraftStatus? status,
            Guid? contactId,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(
                    new EmailDraftFilterRequest(status, contactId),
                    cancellationToken))))
            .WithName("ListEmailDrafts")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var draft = await service.GetByIdAsync(id, cancellationToken);
                return draft is null
                    ? ApiEndpoint.NotFound("Email draft was not found.")
                    : Results.Ok(draft);
            }))
            .WithName("GetEmailDraft")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateEmailDraftRequest request,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateEmailDraft")
            .WithOpenApi();

        group.MapPost("/{id:guid}/approve", async (
            Guid id,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ApproveAsync(id, cancellationToken))))
            .WithName("ApproveEmailDraft")
            .WithOpenApi();

        group.MapPost("/{id:guid}/cancel", async (
            Guid id,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.CancelAsync(id, cancellationToken))))
            .WithName("CancelEmailDraft")
            .WithOpenApi();

        group.MapPost("/{id:guid}/send", async (
            Guid id,
            IEmailDraftService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.SendApprovedDraftAsync(id, cancellationToken))))
            .WithName("SendApprovedEmailDraft")
            .WithOpenApi();

        return endpoints;
    }
}
