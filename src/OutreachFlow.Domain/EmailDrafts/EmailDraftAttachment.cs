using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.EmailDrafts;

public sealed class EmailDraftAttachment
{
    private EmailDraftAttachment()
    {
    }

    public EmailDraftAttachment(Guid emailDraftId, Guid attachmentAssetId)
    {
        if (emailDraftId == Guid.Empty)
        {
            throw new DomainException("Email draft id is required.");
        }

        if (attachmentAssetId == Guid.Empty)
        {
            throw new DomainException("Attachment asset id is required.");
        }

        EmailDraftId = emailDraftId;
        AttachmentAssetId = attachmentAssetId;
    }

    public Guid EmailDraftId { get; private set; }

    public Guid AttachmentAssetId { get; private set; }
}
