using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.Templates;

public sealed record TemplateContext(
    Contact Contact,
    Organization? Organization,
    SenderProfile SenderProfile);
