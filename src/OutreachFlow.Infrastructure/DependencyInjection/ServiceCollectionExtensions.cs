using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tags;
using OutreachFlow.Infrastructure.Persistence;
using OutreachFlow.Infrastructure.Persistence.Queries;
using OutreachFlow.Infrastructure.Persistence.Repositories;
using OutreachFlow.Infrastructure.Storage;

namespace OutreachFlow.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OutreachFlow");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'OutreachFlow' is not configured.");
        }

        services.AddDbContext<OutreachFlowDbContext>(options =>
            options.UseSqlite(connectionString));

        services.Configure<AttachmentStorageOptions>(options =>
        {
            var configuredRootPath = configuration[$"{AttachmentStorageOptions.SectionName}:RootPath"];

            if (!string.IsNullOrWhiteSpace(configuredRootPath))
            {
                options.RootPath = configuredRootPath;
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ISenderProfileRepository, SenderProfileRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IAttachmentAssetRepository, AttachmentAssetRepository>();
        services.AddScoped<IAttachmentFileStorage, LocalAttachmentFileStorage>();
        services.AddScoped<IContactLookupService, ContactLookupService>();

        return services;
    }
}
