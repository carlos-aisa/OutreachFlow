namespace OutreachFlow.Application.Templates;

public static class TemplateVariableRegistry
{
    private static readonly IReadOnlyList<TemplateVariableRegistration> Registrations =
    [
        new(
            new TemplateVariableDefinition(
                "contact.displayName",
                "Contact display name.",
                "{{contact.displayName}}"),
            context => context.Contact.DisplayName),
        new(
            new TemplateVariableDefinition(
                "contact.email",
                "Contact email address.",
                "{{contact.email}}"),
            context => context.Contact.Email),
        new(
            new TemplateVariableDefinition(
                "contact.role",
                "Contact role or job title.",
                "{{contact.role}}"),
            context => context.Contact.Role),
        new(
            new TemplateVariableDefinition(
                "organization.name",
                "Associated organization name.",
                "{{organization.name}}"),
            context => context.Organization?.Name),
        new(
            new TemplateVariableDefinition(
                "organization.city",
                "Associated organization city.",
                "{{organization.city}}"),
            context => context.Organization?.City),
        new(
            new TemplateVariableDefinition(
                "organization.province",
                "Associated organization province.",
                "{{organization.province}}"),
            context => context.Organization?.Province),
        new(
            new TemplateVariableDefinition(
                "sender.name",
                "Sender profile display name.",
                "{{sender.name}}"),
            context => context.SenderProfile.Name),
        new(
            new TemplateVariableDefinition(
                "sender.email",
                "Sender profile email address.",
                "{{sender.email}}"),
            context => context.SenderProfile.Email),
        new(
            new TemplateVariableDefinition(
                "sender.phone",
                "Sender profile phone number.",
                "{{sender.phone}}"),
            context => context.SenderProfile.Phone),
        new(
            new TemplateVariableDefinition(
                "sender.organizationName",
                "Sender organization name.",
                "{{sender.organizationName}}"),
            context => context.SenderProfile.OrganizationName),
        new(
            new TemplateVariableDefinition(
                "sender.website",
                "Sender website.",
                "{{sender.website}}"),
            context => context.SenderProfile.Website),
        new(
            new TemplateVariableDefinition(
                "sender.signature",
                "Sender signature.",
                "{{sender.signature}}"),
            context => context.SenderProfile.Signature)
    ];

    private static readonly Dictionary<string, TemplateVariableRegistration> RegistrationsByName =
        Registrations.ToDictionary(
            registration => registration.Definition.Name,
            registration => registration,
            StringComparer.Ordinal);

    private static readonly IReadOnlyList<TemplateVariableDefinition> Definitions =
        Registrations.Select(registration => registration.Definition).ToArray();

    public static IReadOnlyList<TemplateVariableDefinition> ListSupported()
    {
        return Definitions;
    }

    public static bool TryResolve(
        string variableName,
        TemplateContext context,
        out string? value)
    {
        ArgumentNullException.ThrowIfNull(variableName);
        ArgumentNullException.ThrowIfNull(context);

        if (!RegistrationsByName.TryGetValue(variableName, out var registration))
        {
            value = null;
            return false;
        }

        value = registration.Resolver(context);
        return true;
    }

    private sealed record TemplateVariableRegistration(
        TemplateVariableDefinition Definition,
        Func<TemplateContext, string?> Resolver);
}
