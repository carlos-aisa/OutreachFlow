using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.EmailTemplates;

public sealed class EmailTemplateAttachment
{
    private EmailTemplateAttachment()
    {
    }

    public EmailTemplateAttachment(Guid emailTemplateId, Guid attachmentAssetId)
    {
        if (emailTemplateId == Guid.Empty)
        {
            throw new DomainException("Email template id is required.");
        }

        if (attachmentAssetId == Guid.Empty)
        {
            throw new DomainException("Attachment asset id is required.");
        }

        EmailTemplateId = emailTemplateId;
        AttachmentAssetId = attachmentAssetId;
    }

    public Guid EmailTemplateId { get; private set; }

    public Guid AttachmentAssetId { get; private set; }
}
