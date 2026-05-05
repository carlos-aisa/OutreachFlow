using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Organizations;

namespace OutreachFlow.Api.Endpoints;

public static class OrganizationEndpoints
{
    public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/organizations")
            .WithTags("Organizations");

        group.MapGet("", async (IOrganizationService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.ListAsync(cancellationToken))))
            .WithName("ListOrganizations")
            .WithOpenApi();

        group.MapPost("", async (
            CreateOrganizationRequest request,
            IOrganizationService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var organization = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/organizations/{organization.Id}", organization);
            }))
            .WithName("CreateOrganization")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IOrganizationService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var organization = await service.GetByIdAsync(id, cancellationToken);
                return organization is null
                    ? ApiEndpoint.NotFound("Organization was not found.")
                    : Results.Ok(organization);
            }))
            .WithName("GetOrganization")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateOrganizationRequest request,
            IOrganizationService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateOrganization")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IOrganizationService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeleteOrganization")
            .WithOpenApi();

        return endpoints;
    }
}
