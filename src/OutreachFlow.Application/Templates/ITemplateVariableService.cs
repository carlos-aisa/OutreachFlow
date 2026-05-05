namespace OutreachFlow.Application.Templates;

public interface ITemplateVariableService
{
    IReadOnlyList<TemplateVariableDto> ListSupportedVariables();
}
