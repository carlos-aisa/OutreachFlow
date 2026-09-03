using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Organizations;

namespace OutreachFlow.Api.Endpoints;

public static class OrganizationTypeEndpoints
{
    public static IEndpointRouteBuilder MapOrganizationTypeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/organization-types")
            .WithTags("OrganizationTypes");

        group.MapGet("", async (IOrganizationTypeService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(cancellationToken))))
            .WithName("ListOrganizationTypes")
            .WithOpenApi();

        group.MapPost("", async (
            CreateOrganizationTypeRequest request,
            IOrganizationTypeService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var organizationType = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/organization-types/{organizationType.Id}", organizationType);
            }))
            .WithName("CreateOrganizationType")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IOrganizationTypeService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var organizationType = await service.GetByIdAsync(id, cancellationToken);
                return organizationType is null
                    ? ApiEndpoint.NotFound("Organization type was not found.")
                    : Results.Ok(organizationType);
            }))
            .WithName("GetOrganizationType")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateOrganizationTypeRequest request,
            IOrganizationTypeService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateOrganizationType")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IOrganizationTypeService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeleteOrganizationType")
            .WithOpenApi();

        return endpoints;
    }
}
