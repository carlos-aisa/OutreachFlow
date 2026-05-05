using System.Text.RegularExpressions;

using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Application.Templates;

public sealed class TemplateRenderer : ITemplateRenderer
{
    private static readonly Regex TokenPattern = new(
        "{{\\s*(?<token>[^{}]+?)\\s*}}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250));

    public RenderedEmail Render(
        EmailTemplate emailTemplate,
        TemplateContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emailTemplate);
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        var missingVariables = new HashSet<string>(StringComparer.Ordinal);
        var unknownVariables = new HashSet<string>(StringComparer.Ordinal);

        var renderedSubject = RenderTemplateText(
            emailTemplate.SubjectTemplate,
            context,
            missingVariables,
            unknownVariables);

        var renderedBody = RenderTemplateText(
            emailTemplate.BodyTemplate,
            context,
            missingVariables,
            unknownVariables);

        var hasUnresolvedTokens = ContainsTemplateTokens(renderedSubject) || ContainsTemplateTokens(renderedBody);

        var missingVariableList = missingVariables
            .OrderBy(variableName => variableName, StringComparer.Ordinal)
            .ToArray();

        var unknownVariableList = unknownVariables
            .OrderBy(variableName => variableName, StringComparer.Ordinal)
            .ToArray();

        var hasErrors = hasUnresolvedTokens
            || missingVariableList.Length > 0
            || unknownVariableList.Length > 0;

        return new RenderedEmail(
            renderedSubject,
            renderedBody,
            missingVariableList,
            unknownVariableList,
            hasErrors);
    }

    private static string RenderTemplateText(
        string templateText,
        TemplateContext context,
        HashSet<string> missingVariables,
        HashSet<string> unknownVariables)
    {
        return TokenPattern.Replace(templateText, match =>
        {
            var variableName = match.Groups["token"].Value.Trim();

            if (string.IsNullOrWhiteSpace(variableName))
            {
                return match.Value;
            }

            if (!TemplateVariableRegistry.TryResolve(variableName, context, out var resolvedValue))
            {
                unknownVariables.Add(variableName);
                return match.Value;
            }

            if (string.IsNullOrWhiteSpace(resolvedValue))
            {
                missingVariables.Add(variableName);
                return match.Value;
            }

            return resolvedValue;
        });
    }

    private static bool ContainsTemplateTokens(string value)
    {
        return TokenPattern.IsMatch(value);
    }
}
