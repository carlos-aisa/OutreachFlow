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

        return endpoints;
    }
}
