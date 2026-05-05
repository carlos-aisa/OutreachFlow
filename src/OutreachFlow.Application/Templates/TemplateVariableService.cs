namespace OutreachFlow.Application.Templates;

public sealed class TemplateVariableService : ITemplateVariableService
{
    private static readonly IReadOnlyList<TemplateVariableDto> SupportedVariables =
    [
        new("contact.displayName", "Contact display name.", "{{contact.displayName}}"),
        new("contact.email", "Contact email address.", "{{contact.email}}"),
        new("contact.role", "Contact role or job title.", "{{contact.role}}"),
        new("organization.name", "Associated organization name.", "{{organization.name}}"),
        new("organization.city", "Associated organization city.", "{{organization.city}}"),
        new("organization.province", "Associated organization province.", "{{organization.province}}"),
        new("sender.name", "Sender profile display name.", "{{sender.name}}"),
        new("sender.email", "Sender profile email address.", "{{sender.email}}"),
        new("sender.phone", "Sender profile phone number.", "{{sender.phone}}"),
        new("sender.organizationName", "Sender organization name.", "{{sender.organizationName}}"),
        new("sender.website", "Sender website.", "{{sender.website}}"),
        new("sender.signature", "Sender signature.", "{{sender.signature}}")
    ];

    public IReadOnlyList<TemplateVariableDto> ListSupportedVariables()
    {
        return SupportedVariables;
    }
}
