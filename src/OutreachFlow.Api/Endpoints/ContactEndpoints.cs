using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Api.Endpoints;

public static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContactEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/contacts")
            .WithTags("Contacts");

        group.MapGet("", async (
            string? search,
            Guid? tagId,
            ContactStatus? status,
            bool? doNotContact,
            Guid? organizationId,
            DateTimeOffset? lastContactedFrom,
            DateTimeOffset? lastContactedTo,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var filter = new ContactFilterRequest(
                    search,
                    tagId,
                    status,
                    doNotContact,
                    organizationId,
                    lastContactedFrom,
                    lastContactedTo);

                return Results.Ok(await service.ListAsync(filter, cancellationToken));
            }))
            .WithName("ListContacts")
            .WithOpenApi();

        group.MapPost("", async (
            CreateContactRequest request,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var contact = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/contacts/{contact.Id}", contact);
            }))
            .WithName("CreateContact")
            .WithOpenApi();

        group.MapGet("/{id:guid}", async (
            Guid id,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var contact = await service.GetByIdAsync(id, cancellationToken);
                return contact is null
                    ? ApiEndpoint.NotFound("Contact was not found.")
                    : Results.Ok(contact);
            }))
            .WithName("GetContact")
            .WithOpenApi();

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateContactRequest request,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
                Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateContact")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.DeleteAsync(id, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("DeleteContact")
            .WithOpenApi();

        group.MapPost("/{id:guid}/tags/{tagId:guid}", async (
            Guid id,
            Guid tagId,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.AssignTagAsync(id, tagId, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("AssignContactTag")
            .WithOpenApi();

        group.MapDelete("/{id:guid}/tags/{tagId:guid}", async (
            Guid id,
            Guid tagId,
            IContactService service,
            CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                await service.RemoveTagAsync(id, tagId, cancellationToken);
                return Results.NoContent();
            }))
            .WithName("RemoveContactTag")
            .WithOpenApi();

        return endpoints;
    }
}
