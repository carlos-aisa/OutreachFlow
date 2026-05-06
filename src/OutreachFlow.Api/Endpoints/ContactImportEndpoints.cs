using OutreachFlow.Api.Errors;
using OutreachFlow.Application.ContactImports;

namespace OutreachFlow.Api.Endpoints;

public static class ContactImportEndpoints
{
    public static IEndpointRouteBuilder MapContactImportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/contact-imports")
            .WithTags("ContactImports");

        group.MapPost("/preview", async (
            ContactImportPreviewRequest request,
            IContactImportService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.PreviewAsync(request, cancellationToken))))
            .WithName("PreviewContactImport")
            .WithOpenApi();

        group.MapPost("/commit", async (
            ContactImportCommitRequest request,
            IContactImportService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.CommitAsync(request, cancellationToken))))
            .WithName("CommitContactImport")
            .WithOpenApi();

        group.MapGet("/jobs", async (
            int? limit,
            IContactImportService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListJobsAsync(limit, cancellationToken))))
            .WithName("ListContactImportJobs")
            .WithOpenApi();

        return endpoints;
    }
}
