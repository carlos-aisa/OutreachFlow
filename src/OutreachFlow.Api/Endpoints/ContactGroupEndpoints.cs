using OutreachFlow.Api.Errors;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Domain.ContactGroups;

namespace OutreachFlow.Api.Endpoints;

public static class ContactGroupEndpoints
{
    public static IEndpointRouteBuilder MapContactGroupEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/contact-groups").WithTags("Contact groups");

        group.MapGet("", async (IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.ListAsync(cancellationToken))))
            .WithName("ListContactGroups").WithOpenApi();

        group.MapPost("", async (CreateContactGroupRequest request, IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var contactGroup = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/contact-groups/{contactGroup.Id}", contactGroup);
            }))
            .WithName("CreateContactGroup").WithOpenApi();

        group.MapGet("/{id:guid}", async (Guid id, IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.GetByIdAsync(id, cancellationToken))))
            .WithName("GetContactGroup").WithOpenApi();

        group.MapPut("/{id:guid}", async (Guid id, UpdateContactGroupRequest request, IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateContactGroup").WithOpenApi();

        group.MapDelete("/{id:guid}", async (Guid id, IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => { await service.DeleteAsync(id, cancellationToken); return Results.NoContent(); }))
            .WithName("DeleteContactGroup").WithOpenApi();

        group.MapGet("/{id:guid}/members", async (Guid id, IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.ListMembersAsync(id, cancellationToken))))
            .WithName("ListContactGroupMembers").WithOpenApi();

        group.MapPut("/{id:guid}/members/{contactId:guid}/membership-override", async (Guid id, Guid contactId, ContactGroupOverrideType type, IContactGroupService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.SetOverrideAsync(id, contactId, type, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("SetContactGroupMemberOverride").WithOpenApi();

        return endpoints;
    }
}
