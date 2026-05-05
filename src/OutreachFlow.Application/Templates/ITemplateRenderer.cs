using OutreachFlow.Domain.EmailTemplates;

namespace OutreachFlow.Application.Templates;

public interface ITemplateRenderer
{
    RenderedEmail Render(
        EmailTemplate emailTemplate,
        TemplateContext context,
        CancellationToken cancellationToken = default);
}
