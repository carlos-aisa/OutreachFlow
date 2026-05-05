using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tags;
using OutreachFlow.Infrastructure.EmailSending;
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
        services.Configure<EmailSendingOptions>(options =>
        {
            options.Provider = configuration[$"{EmailSendingOptions.SectionName}:Provider"] ?? options.Provider;

            var equivalentWindowValue = configuration[$"{EmailSendingOptions.SectionName}:EquivalentEmailWindowHours"];
            if (int.TryParse(equivalentWindowValue, out var equivalentWindowHours) && equivalentWindowHours > 0)
            {
                options.EquivalentEmailWindowHours = equivalentWindowHours;
            }

            var fakeFailureKeyword = configuration[$"{EmailSendingOptions.SectionName}:FakeFailureKeyword"];
            if (!string.IsNullOrWhiteSpace(fakeFailureKeyword))
            {
                options.FakeFailureKeyword = fakeFailureKeyword;
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ISenderProfileRepository, SenderProfileRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEmailDraftRepository, EmailDraftRepository>();
        services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
        services.AddScoped<IAttachmentAssetRepository, AttachmentAssetRepository>();
        services.AddScoped<IAttachmentFileStorage, LocalAttachmentFileStorage>();
        services.AddScoped<IContactLookupService, ContactLookupService>();
        services.AddScoped<FakeEmailSender>();
        services.AddScoped<IEmailSender>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSendingOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.Provider) ||
                options.Provider.Equals("Fake", StringComparison.OrdinalIgnoreCase))
            {
                return serviceProvider.GetRequiredService<FakeEmailSender>();
            }

            throw new InvalidOperationException(
                $"Email sender provider '{options.Provider}' is not supported in this version.");
        });
        services.AddSingleton<IEmailSendingPolicy>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSendingOptions>>().Value;
            var hours = options.EquivalentEmailWindowHours <= 0 ? 168 : options.EquivalentEmailWindowHours;
            return new ConfiguredEmailSendingPolicy(TimeSpan.FromHours(hours));
        });

        return services;
    }
}
