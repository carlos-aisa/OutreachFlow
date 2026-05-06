using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tags;
using OutreachFlow.Infrastructure.EmailSending;
using OutreachFlow.Infrastructure.FollowUps;
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
        services.Configure<SmtpEmailSenderOptions>(options =>
        {
            options.Host = configuration[$"{SmtpEmailSenderOptions.SectionName}:Host"] ?? options.Host;

            var portValue = configuration[$"{SmtpEmailSenderOptions.SectionName}:Port"];
            if (int.TryParse(portValue, out var port) && port > 0)
            {
                options.Port = port;
            }

            var useSslValue = configuration[$"{SmtpEmailSenderOptions.SectionName}:UseSsl"];
            if (bool.TryParse(useSslValue, out var useSsl))
            {
                options.UseSsl = useSsl;
            }

            options.Username = configuration[$"{SmtpEmailSenderOptions.SectionName}:Username"] ?? options.Username;
            options.Password = configuration[$"{SmtpEmailSenderOptions.SectionName}:Password"] ?? options.Password;

            var timeoutValue = configuration[$"{SmtpEmailSenderOptions.SectionName}:TimeoutSeconds"];
            if (int.TryParse(timeoutValue, out var timeoutSeconds) && timeoutSeconds > 0)
            {
                options.TimeoutSeconds = timeoutSeconds;
            }
        });
        services.Configure<FollowUpAutomationOptions>(options =>
        {
            var autoCreateValue = configuration[$"{FollowUpAutomationOptions.SectionName}:AutoCreateAfterSuccessfulSend"];
            if (bool.TryParse(autoCreateValue, out var autoCreateAfterSuccessfulSend))
            {
                options.AutoCreateAfterSuccessfulSend = autoCreateAfterSuccessfulSend;
            }

            var dueDaysValue = configuration[$"{FollowUpAutomationOptions.SectionName}:DueDaysAfterSend"];
            if (int.TryParse(dueDaysValue, out var dueDaysAfterSend) && dueDaysAfterSend > 0)
            {
                options.DueDaysAfterSend = dueDaysAfterSend;
            }

            var defaultTypeValue = configuration[$"{FollowUpAutomationOptions.SectionName}:DefaultType"];
            if (Enum.TryParse<OutreachFlow.Domain.FollowUps.FollowUpTaskType>(
                defaultTypeValue,
                ignoreCase: true,
                out var defaultType))
            {
                options.DefaultType = defaultType;
            }
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactActivityRepository, ContactActivityRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ISenderProfileRepository, SenderProfileRepository>();
        services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
        services.AddScoped<IEmailDraftRepository, EmailDraftRepository>();
        services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
        services.AddScoped<IFollowUpTaskRepository, FollowUpTaskRepository>();
        services.AddScoped<IAttachmentAssetRepository, AttachmentAssetRepository>();
        services.AddScoped<IAttachmentFileStorage, LocalAttachmentFileStorage>();
        services.AddScoped<IContactLookupService, ContactLookupService>();
        services.AddScoped<FakeEmailSender>();
        services.AddScoped<SmtpEmailSender>();
        services.AddSingleton<ISmtpTransportFactory, SystemSmtpTransportFactory>();
        services.AddScoped<IEmailSender>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailSendingOptions>>().Value;

            if (string.IsNullOrWhiteSpace(options.Provider) ||
                options.Provider.Equals("Fake", StringComparison.OrdinalIgnoreCase))
            {
                return serviceProvider.GetRequiredService<FakeEmailSender>();
            }

            if (options.Provider.Equals("SMTP", StringComparison.OrdinalIgnoreCase))
            {
                var smtpOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SmtpEmailSenderOptions>>().Value;
                SmtpEmailSenderOptionsValidator.ValidateConfigured(smtpOptions);
                return serviceProvider.GetRequiredService<SmtpEmailSender>();
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
        services.AddSingleton<IFollowUpAutomationPolicy, ConfiguredFollowUpAutomationPolicy>();

        return services;
    }
}
