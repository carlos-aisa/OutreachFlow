using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tags;

namespace OutreachFlow.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ITagService, TagService>();

        return services;
    }
}
