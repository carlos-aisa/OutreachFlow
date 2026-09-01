using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Campaigns;

namespace OutreachFlow.Api.Endpoints;

public static class CampaignEndpoints
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/campaigns").WithTags("Campaigns");

        group.MapGet("", async (ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.ListAsync(cancellationToken))))
            .WithName("ListCampaigns").WithOpenApi();

        group.MapPost("", async (CreateCampaignRequest request, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var campaign = await service.CreateAsync(request, cancellationToken);
                return Results.Created($"/api/v1/campaigns/{campaign.Id}", campaign);
            }))
            .WithName("CreateCampaign").WithOpenApi();

        group.MapGet("/{id:guid}", async (Guid id, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.GetByIdAsync(id, cancellationToken))))
            .WithName("GetCampaign").WithOpenApi();

        group.MapPut("/{id:guid}", async (Guid id, UpdateCampaignRequest request, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.UpdateAsync(id, request, cancellationToken))))
            .WithName("UpdateCampaign").WithOpenApi();

        group.MapPost("/{id:guid}/audience-groups/{contactGroupId:guid}", async (Guid id, Guid contactGroupId, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.AddAudienceGroupAsync(id, contactGroupId, cancellationToken))))
            .WithName("AddCampaignAudienceGroup").WithOpenApi();

        group.MapDelete("/{id:guid}/audience-groups/{contactGroupId:guid}", async (Guid id, Guid contactGroupId, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.RemoveAudienceGroupAsync(id, contactGroupId, cancellationToken))))
            .WithName("RemoveCampaignAudienceGroup").WithOpenApi();

        group.MapPost("/{id:guid}/close", async (Guid id, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.CloseAsync(id, cancellationToken))))
            .WithName("CloseCampaign").WithOpenApi();

        group.MapPost("/{id:guid}/reopen", async (Guid id, ICampaignService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.ReopenAsync(id, cancellationToken))))
            .WithName("ReopenCampaign").WithOpenApi();

        return endpoints;
    }
}
