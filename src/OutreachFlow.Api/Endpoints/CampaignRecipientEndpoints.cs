using OutreachFlow.Api.Errors;
using OutreachFlow.Application.Campaigns;

namespace OutreachFlow.Api.Endpoints;

public static class CampaignRecipientEndpoints
{
    public static IEndpointRouteBuilder MapCampaignRecipientEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/campaigns/{campaignId:guid}").WithTags("Campaign recipients");

        group.MapGet("/candidates", async (Guid campaignId, ICampaignRecipientService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.DiscoverCandidatesAsync(campaignId, cancellationToken))))
            .WithName("DiscoverCampaignRecipientCandidates").WithOpenApi();

        group.MapGet("/recipients", async (Guid campaignId, ICampaignRecipientService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.ListAsync(campaignId, cancellationToken))))
            .WithName("ListCampaignRecipients").WithOpenApi();

        group.MapPost("/recipients/{contactId:guid}", async (Guid campaignId, Guid contactId, ICampaignRecipientService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () =>
            {
                var recipient = await service.IncorporateAsync(campaignId, contactId, cancellationToken);
                return Results.Created($"/api/v1/campaigns/{campaignId}/recipients", recipient);
            }))
            .WithName("IncorporateCampaignRecipient").WithOpenApi();

        group.MapPost("/recipients/generate-drafts", async (Guid campaignId, GenerateCampaignDraftsRequest request, ICampaignRecipientService service, CancellationToken cancellationToken) =>
            await ApiEndpoint.HandleAsync(async () => Results.Ok(await service.GenerateDraftsAsync(campaignId, request, cancellationToken))))
            .WithName("GenerateCampaignRecipientDrafts").WithOpenApi();

        return endpoints;
    }
}
