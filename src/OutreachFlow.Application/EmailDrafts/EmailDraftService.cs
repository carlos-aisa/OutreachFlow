using System.Text.Json;
using System.Text.RegularExpressions;

using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Templates;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailMessages;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.EmailDrafts;

public sealed class EmailDraftService(
    IContactRepository contactRepository,
    IOrganizationRepository organizationRepository,
    IEmailTemplateRepository emailTemplateRepository,
    ISenderProfileRepository senderProfileRepository,
    IAttachmentAssetRepository attachmentAssetRepository,
    IEmailDraftRepository emailDraftRepository,
    IEmailMessageRepository emailMessageRepository,
    IEmailSender emailSender,
    IEmailSendingPolicy emailSendingPolicy,
    IContactActivityService contactActivityService,
    ITemplateRenderer templateRenderer,
    IUnitOfWork unitOfWork)
    : IEmailDraftService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly Regex UnresolvedTokenRegex = new(
        @"\{\{[^{}]+\}\}",
        RegexOptions.Compiled);

    public async Task<GenerateEmailDraftsResult> GenerateAsync(
        GenerateEmailDraftsRequest request,
        CancellationToken cancellationToken = default)
    {
        var emailTemplate = await emailTemplateRepository.GetByIdAsync(request.TemplateId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email template was not found.");

        if (!emailTemplate.IsActive)
        {
            throw new ApplicationValidationException("Inactive email templates cannot be used for draft generation.");
        }

        var senderProfile = await senderProfileRepository.GetByIdAsync(request.SenderProfileId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Sender profile was not found.");

        if (!senderProfile.IsActive)
        {
            throw new ApplicationValidationException("Inactive sender profiles cannot be used for draft generation.");
        }

        var contactFilter = new ContactFilterRequest(
            request.Search,
            request.TagId,
            request.Status,
            request.DoNotContact,
            request.OrganizationId,
            request.LastContactedFrom,
            request.LastContactedTo);

        var contacts = await contactRepository.ListAsync(contactFilter, cancellationToken);
        var requestedContacts = contacts.Count;

        var optionalAttachmentIds = request.AttachmentAssetIds?
            .Where(attachmentId => attachmentId != Guid.Empty)
            .Distinct()
            .ToArray() ?? [];

        var optionalAttachments = await ResolveActiveAttachmentsAsync(
            optionalAttachmentIds,
            "Selected attachment was not found.",
            cancellationToken);

        var defaultAttachmentIds = emailTemplate.DefaultAttachments
            .Select(defaultAttachment => defaultAttachment.AttachmentAssetId)
            .Distinct()
            .ToArray();

        var defaultAttachments = await ResolveActiveAttachmentsAsync(
            defaultAttachmentIds,
            "Template default attachment was not found.",
            cancellationToken);

        var allAttachments = defaultAttachments
            .Concat(optionalAttachments)
            .GroupBy(attachmentAsset => attachmentAsset.Id)
            .Select(group => group.First())
            .ToArray();

        var drafts = new List<EmailDraft>();
        var skippedContacts = new List<SkippedDraftContactDto>();
        var organizationCache = new Dictionary<Guid, Organization?>();

        foreach (var contact in contacts)
        {
            if (contact.DoNotContact)
            {
                skippedContacts.Add(new SkippedDraftContactDto(
                    contact.Id,
                    contact.DisplayName,
                    contact.Email,
                    "Contact is marked as Do Not Contact."));
                continue;
            }

            if (string.IsNullOrWhiteSpace(contact.Email))
            {
                skippedContacts.Add(new SkippedDraftContactDto(
                    contact.Id,
                    contact.DisplayName,
                    contact.Email,
                    "Contact does not have a valid email address."));
                continue;
            }

            var organization = await ResolveOrganizationAsync(contact.OrganizationId, organizationCache, cancellationToken);
            var renderedEmail = templateRenderer.Render(
                emailTemplate,
                new TemplateContext(contact, organization, senderProfile),
                cancellationToken);

            var draft = EmailDraft.CreateGenerated(
                contact.Id,
                contact.OrganizationId,
                emailTemplate.Id,
                senderProfile.Id,
                renderedEmail.Subject,
                renderedEmail.Body,
                renderedEmail.HasErrors,
                SerializeVariables(renderedEmail.MissingVariables),
                SerializeVariables(renderedEmail.UnknownVariables),
                DateTimeOffset.UtcNow);

            foreach (var attachmentAsset in allAttachments)
            {
                draft.AssignAttachment(attachmentAsset, DateTimeOffset.UtcNow);
            }

            drafts.Add(draft);
            await contactActivityService.RecordAsync(new CreateContactActivityRequest(
                draft.ContactId,
                draft.OrganizationId,
                ContactActivityType.EmailDraftCreated,
                Subject: draft.Subject,
                BodyPreview: BuildBodyPreview(draft.Body),
                MetadataJson: null),
                cancellationToken);
        }

        if (drafts.Count > 0)
        {
            await emailDraftRepository.AddRangeAsync(drafts, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var draftDtos = drafts
            .Select(draft =>
            {
                var contact = contacts.First(item => item.Id == draft.ContactId);
                return Map(draft, contact);
            })
            .ToArray();

        return new GenerateEmailDraftsResult(
            requestedContacts,
            draftDtos.Length,
            skippedContacts.Count,
            draftDtos,
            skippedContacts);
    }

    public async Task<IReadOnlyList<EmailDraftDto>> ListAsync(
        EmailDraftFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        var drafts = await emailDraftRepository.ListAsync(filter, cancellationToken);
        var draftDtos = new List<EmailDraftDto>(drafts.Count);

        foreach (var draft in drafts)
        {
            var contact = await contactRepository.GetByIdAsync(draft.ContactId, cancellationToken)
                ?? throw new ApplicationNotFoundException("Draft contact was not found.");

            draftDtos.Add(Map(draft, contact));
        }

        return draftDtos;
    }

    public async Task<EmailDraftDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var draft = await emailDraftRepository.GetByIdAsync(id, cancellationToken);

        if (draft is null)
        {
            return null;
        }

        var contact = await contactRepository.GetByIdAsync(draft.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Draft contact was not found.");

        return Map(draft, contact);
    }

    public async Task<EmailDraftDto> UpdateAsync(
        Guid id,
        UpdateEmailDraftRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var draft = await emailDraftRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email draft was not found.");

        try
        {
            draft.UpdateContent(request.Subject, request.Body, DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var contact = await contactRepository.GetByIdAsync(draft.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Draft contact was not found.");

        return Map(draft, contact);
    }

    public async Task<EmailDraftDto> ApproveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var draft = await emailDraftRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email draft was not found.");

        try
        {
            draft.Approve(DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var contact = await contactRepository.GetByIdAsync(draft.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Draft contact was not found.");

        return Map(draft, contact);
    }

    public async Task<EmailDraftDto> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var draft = await emailDraftRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email draft was not found.");

        try
        {
            draft.Cancel(DateTimeOffset.UtcNow);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var contact = await contactRepository.GetByIdAsync(draft.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Draft contact was not found.");

        return Map(draft, contact);
    }

    public async Task<EmailDraftDto> SendApprovedDraftAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var draft = await emailDraftRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Email draft was not found.");

        var contact = await contactRepository.GetByIdAsync(draft.ContactId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Draft contact was not found.");

        var senderProfile = await senderProfileRepository.GetByIdAsync(draft.SenderProfileId, cancellationToken)
            ?? throw new ApplicationNotFoundException("Sender profile was not found.");

        EnsureDraftCanBeSent(draft);

        if (contact.DoNotContact)
        {
            throw new ApplicationValidationException("Do Not Contact recipients cannot receive emails.");
        }

        if (string.IsNullOrWhiteSpace(contact.Email))
        {
            throw new ApplicationValidationException("Contact does not have a valid recipient email address.");
        }

        if (!senderProfile.IsActive)
        {
            throw new ApplicationValidationException("Inactive sender profiles cannot be used for email sending.");
        }

        var now = DateTimeOffset.UtcNow;
        var hasEquivalentRecentSend = await emailMessageRepository.ExistsEquivalentSentEmailAsync(
            contact.Id,
            draft.Subject,
            now.Subtract(emailSendingPolicy.EquivalentEmailWindow),
            cancellationToken);

        if (hasEquivalentRecentSend)
        {
            throw new ApplicationValidationException(
                "An equivalent email was already sent to this contact recently.");
        }

        var attachments = await ResolveDraftAttachmentsAsync(draft, cancellationToken);
        var sendCommand = new SendEmailCommand(
            contact.Email,
            draft.Subject,
            draft.Body,
            new EmailSenderPayload(
                senderProfile.Id,
                senderProfile.Name,
                senderProfile.Email,
                senderProfile.Phone,
                senderProfile.OrganizationName,
                senderProfile.Website,
                senderProfile.Signature),
            attachments
                .Select(attachmentAsset => new EmailAttachmentPayload(
                    attachmentAsset.Id,
                    attachmentAsset.Name,
                    attachmentAsset.FileName,
                    attachmentAsset.ContentType,
                    attachmentAsset.StoragePath,
                    attachmentAsset.SizeBytes))
                .ToArray(),
            new Dictionary<string, string>
            {
                ["draftId"] = draft.Id.ToString(),
                ["contactId"] = contact.Id.ToString()
            });

        var sendResult = await emailSender.SendAsync(sendCommand, cancellationToken);
        var provider = string.IsNullOrWhiteSpace(sendResult.Provider)
            ? "Unknown"
            : sendResult.Provider.Trim();

        EmailMessage emailMessage;

        try
        {
            if (sendResult.Success)
            {
                draft.MarkSent(now);
                contact.MarkContacted(now);
                emailMessage = EmailMessage.CreateSent(
                    contact.Id,
                    draft.OrganizationId,
                    draft.Id,
                    contact.Email,
                    draft.Subject,
                    draft.Body,
                    provider,
                    sendResult.ProviderMessageId,
                    now);
            }
            else
            {
                var failureReason = string.IsNullOrWhiteSpace(sendResult.ErrorMessage)
                    ? "The email provider returned a failure result."
                    : sendResult.ErrorMessage.Trim();

                draft.MarkFailed(failureReason, now);
                emailMessage = EmailMessage.CreateFailed(
                    contact.Id,
                    draft.OrganizationId,
                    draft.Id,
                    contact.Email,
                    draft.Subject,
                    draft.Body,
                    provider,
                    failureReason,
                    now);
            }
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await emailMessageRepository.AddAsync(emailMessage, cancellationToken);
        await contactActivityService.RecordAsync(new CreateContactActivityRequest(
            contact.Id,
            draft.OrganizationId,
            sendResult.Success ? ContactActivityType.EmailSent : ContactActivityType.EmailFailed,
            Subject: draft.Subject,
            BodyPreview: BuildBodyPreview(draft.Body),
            MetadataJson: SerializeActivityMetadata(provider, sendResult.ProviderMessageId, draft.FailureReason)),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(draft, contact);
    }

    private async Task<Organization?> ResolveOrganizationAsync(
        Guid? organizationId,
        Dictionary<Guid, Organization?> organizationCache,
        CancellationToken cancellationToken)
    {
        if (organizationId is null)
        {
            return null;
        }

        if (organizationCache.TryGetValue(organizationId.Value, out var cachedOrganization))
        {
            return cachedOrganization;
        }

        var organization = await organizationRepository.GetByIdAsync(organizationId.Value, cancellationToken);
        organizationCache[organizationId.Value] = organization;
        return organization;
    }

    private async Task<IReadOnlyList<AttachmentAsset>> ResolveActiveAttachmentsAsync(
        IReadOnlyList<Guid> attachmentIds,
        string notFoundMessage,
        CancellationToken cancellationToken)
    {
        var attachments = new List<AttachmentAsset>(attachmentIds.Count);

        foreach (var attachmentId in attachmentIds)
        {
            var attachmentAsset = await attachmentAssetRepository.GetByIdAsync(attachmentId, cancellationToken)
                ?? throw new ApplicationNotFoundException(notFoundMessage);

            if (!attachmentAsset.IsActive)
            {
                throw new ApplicationValidationException("Inactive attachments cannot be used for draft generation.");
            }

            attachments.Add(attachmentAsset);
        }

        return attachments;
    }

    private async Task<IReadOnlyList<AttachmentAsset>> ResolveDraftAttachmentsAsync(
        EmailDraft draft,
        CancellationToken cancellationToken)
    {
        var attachments = new List<AttachmentAsset>(draft.Attachments.Count);

        foreach (var draftAttachment in draft.Attachments)
        {
            var attachmentAsset = await attachmentAssetRepository.GetByIdAsync(
                draftAttachment.AttachmentAssetId,
                cancellationToken);

            if (attachmentAsset is null)
            {
                throw new ApplicationNotFoundException("A draft attachment asset was not found.");
            }

            if (!attachmentAsset.IsActive)
            {
                throw new ApplicationValidationException("Inactive attachments cannot be used for email sending.");
            }

            attachments.Add(attachmentAsset);
        }

        return attachments;
    }

    private static void EnsureDraftCanBeSent(EmailDraft draft)
    {
        if (draft.Status == EmailDraftStatus.Sent)
        {
            throw new ApplicationValidationException("This draft was already sent.");
        }

        if (draft.Status != EmailDraftStatus.Approved)
        {
            throw new ApplicationValidationException("Only approved drafts can be sent.");
        }

        if (draft.HasRenderErrors)
        {
            throw new ApplicationValidationException("Drafts with render errors cannot be sent.");
        }

        if (!string.IsNullOrWhiteSpace(draft.MissingVariablesJson) ||
            !string.IsNullOrWhiteSpace(draft.UnknownVariablesJson))
        {
            throw new ApplicationValidationException("Draft contains unresolved template diagnostics.");
        }

        if (UnresolvedTokenRegex.IsMatch(draft.Subject) || UnresolvedTokenRegex.IsMatch(draft.Body))
        {
            throw new ApplicationValidationException("Draft contains unresolved template variables.");
        }
    }

    private static string BuildBodyPreview(string body)
    {
        if (body.Length <= 240)
        {
            return body;
        }

        return $"{body[..240]}...";
    }

    private static string SerializeActivityMetadata(
        string provider,
        string? providerMessageId,
        string? failureReason)
    {
        return JsonSerializer.Serialize(new
        {
            provider,
            providerMessageId,
            failureReason
        }, JsonOptions);
    }

    private static EmailDraftDto Map(EmailDraft draft, Contact contact)
    {
        return new EmailDraftDto(
            draft.Id,
            draft.ContactId,
            contact.DisplayName,
            contact.Email,
            draft.OrganizationId,
            draft.TemplateId,
            draft.SenderProfileId,
            draft.Subject,
            draft.Body,
            draft.Status,
            draft.HasRenderErrors,
            DeserializeVariables(draft.MissingVariablesJson),
            DeserializeVariables(draft.UnknownVariablesJson),
            draft.Attachments
                .Select(attachment => attachment.AttachmentAssetId)
                .OrderBy(attachmentId => attachmentId)
                .ToArray(),
            draft.CreatedAt,
            draft.UpdatedAt,
            draft.ApprovedAt,
            draft.SentAt,
            draft.FailureReason,
            draft.CancelledAt);
    }

    private static string? SerializeVariables(IReadOnlyList<string> variableNames)
    {
        return variableNames.Count == 0 ? null : JsonSerializer.Serialize(variableNames, JsonOptions);
    }

    private static IReadOnlyList<string> DeserializeVariables(string? variablesJson)
    {
        if (string.IsNullOrWhiteSpace(variablesJson))
        {
            return [];
        }

        var variableNames = JsonSerializer.Deserialize<IReadOnlyList<string>>(variablesJson, JsonOptions);
        return variableNames ?? [];
    }
}
