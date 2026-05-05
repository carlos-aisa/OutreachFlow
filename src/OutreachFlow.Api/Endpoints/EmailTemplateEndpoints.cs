using OutreachFlow.Api.Errors;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Templates;

namespace OutreachFlow.Api.Endpoints;

public static class EmailTemplateEndpoints
{
    public static IEndpointRouteBuilder MapEmailTemplateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/templates")
            .WithTags("EmailTemplates");

        group.MapGet("", async (
            bool? activeOnly,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(activeOnly, cancellationToken))))
            .WithName("ListEmailTemplates")
            .WithOpenApi();

        group.MapPost("", async (
            CreateEmailTemplateRequest request,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var emailTemplate = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/templates/{emailTemplate.Id}", emailTemplate);
            }))
            .WithName("CreateEmailTemplate")
            .WithOpenApi();

        group.MapGet("/variables", (ITemplateVariableService service) =>
            Results.Ok(service.ListSupportedVariables()))
            .WithName("ListTemplateVariables")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var emailTemplate = await service.GetByIdAsync(id, cancellationToken);
                return emailTemplate is null
                    ? ApiEndpoint.NotFound("Email template was not found.")
                    : Results.Ok(emailTemplate);
            }))
            .WithName("GetEmailTemplate")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateEmailTemplateRequest request,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateEmailTemplate")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeactivateAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeactivateEmailTemplate")
            .WithOpenApi();

        group.MapPost("/{id:guid}/attachments/{attachmentId:guid}", async (
            Guid id,
            Guid attachmentId,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.AssignDefaultAttachmentAsync(
                    id,
                    attachmentId,
                    cancellationToken))))
            .WithName("AssignDefaultTemplateAttachment")
            .WithOpenApi();

        group.MapDelete("/{id:guid}/attachments/{attachmentId:guid}", async (
            Guid id,
            Guid attachmentId,
            IEmailTemplateService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.RemoveDefaultAttachmentAsync(
                    id,
                    attachmentId,
                    cancellationToken))))
            .WithName("RemoveDefaultTemplateAttachment")
            .WithOpenApi();

        return endpoints;
    }
}
