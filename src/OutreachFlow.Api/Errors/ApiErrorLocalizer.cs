using Microsoft.Extensions.Localization;
using OutreachFlow.Api;

namespace OutreachFlow.Api.Errors;

public sealed class ApiErrorLocalizer(IStringLocalizer<ApiErrorResource> localizer) : IApiErrorLocalizer
{
    private static readonly Dictionary<string, string> MessageKeyMap = new(StringComparer.Ordinal)
    {
        ["Organization was not found."] = "ApiError.OrganizationNotFound",
        ["Contact was not found."] = "ApiError.ContactNotFound",
        ["Tag was not found."] = "ApiError.TagNotFound",
        ["Sender profile was not found."] = "ApiError.SenderProfileNotFound",
        ["Default sender profile was not found."] = "ApiError.DefaultSenderProfileNotFound",
        ["Email template was not found."] = "ApiError.EmailTemplateNotFound",
        ["Email draft was not found."] = "ApiError.EmailDraftNotFound",
        ["Follow-up task was not found."] = "ApiError.FollowUpTaskNotFound",
        ["Attachment asset was not found."] = "ApiError.AttachmentAssetNotFound",
        ["A contact with this email already exists."] = "ApiError.ContactEmailAlreadyExists",
        ["Tag already exists in this category."] = "ApiError.TagAlreadyExists",
        ["Draft cannot be approved while render errors remain."] = "ApiError.DraftRenderErrors",
        ["Only approved drafts can be sent."] = "ApiError.OnlyApprovedDraftsCanBeSent",
        ["This draft was already sent."] = "ApiError.DraftAlreadySent",
        ["Signature format is required when signature content is provided."] = "ApiError.SignatureFormatRequired",
        ["Signature format is not supported."] = "ApiError.SignatureFormatNotSupported"
    };

    public string Localize(string message)
    {
        if (!MessageKeyMap.TryGetValue(message, out var key))
        {
            return message;
        }

        var localized = localizer[key];
        return localized.ResourceNotFound ? message : localized.Value;
    }
}
