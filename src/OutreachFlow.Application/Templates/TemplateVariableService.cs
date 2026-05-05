namespace OutreachFlow.Application.Templates;

public sealed class TemplateVariableService : ITemplateVariableService
{
    private static readonly IReadOnlyList<TemplateVariableDto> SupportedVariables = TemplateVariableRegistry
        .ListSupported()
        .Select(variable => new TemplateVariableDto(variable.Name, variable.Description, variable.Example))
        .ToArray();

    public IReadOnlyList<TemplateVariableDto> ListSupportedVariables()
    {
        return SupportedVariables;
    }
}
