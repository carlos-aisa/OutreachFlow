using System.Text.Json;

using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Templates;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailDrafts;
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
    ITemplateRenderer templateRenderer,
    IUnitOfWork unitOfWork)
    : IEmailDraftService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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
            draft.UpdatedAt);
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
