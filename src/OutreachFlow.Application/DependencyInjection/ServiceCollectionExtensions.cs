using Microsoft.Extensions.DependencyInjection;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tags;
using OutreachFlow.Application.Templates;
using OutreachFlow.Application.Attachments;

namespace OutreachFlow.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IContactActivityService, ContactActivityService>();
        services.AddScoped<ISenderProfileService, SenderProfileService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<IAttachmentAssetService, AttachmentAssetService>();
        services.AddScoped<IEmailDraftService, EmailDraftService>();
        services.AddSingleton<ITemplateVariableService, TemplateVariableService>();
        services.AddSingleton<ITemplateRenderer, TemplateRenderer>();

        return services;
    }
}
