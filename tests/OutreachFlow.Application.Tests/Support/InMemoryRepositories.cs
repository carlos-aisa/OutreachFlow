using OutreachFlow.Application.Common;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.EmailSending;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Attachments;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.EmailDrafts;
using OutreachFlow.Domain.EmailMessages;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.FollowUps;
using OutreachFlow.Domain.Organizations;
using OutreachFlow.Domain.SenderProfiles;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Application.Tests.Support;

internal sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public int SaveChangesCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCount++;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryOrganizationRepository : IOrganizationRepository
{
    private readonly List<Organization> _organizations = [];

    public IReadOnlyList<Organization> Organizations => _organizations;

    public Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        _organizations.Add(organization);
        return Task.CompletedTask;
    }

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_organizations.FirstOrDefault(organization => organization.Id == id));
    }

    public Task<IReadOnlyList<Organization>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Organization>>(
            _organizations.OrderBy(organization => organization.Name).ToArray());
    }

    public void Remove(Organization organization)
    {
        _organizations.Remove(organization);
    }
}

internal sealed class InMemoryTagRepository : ITagRepository
{
    private readonly List<Tag> _tags = [];

    public IReadOnlyList<Tag> Tags => _tags;

    public Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _tags.Add(tag);
        return Task.CompletedTask;
    }

    public Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tags.FirstOrDefault(tag => tag.Id == id));
    }

    public Task<Tag?> GetByNameAsync(
        string name,
        string? category,
        CancellationToken cancellationToken = default)
    {
        var normalizedName = NormalizeKey(name);
        var normalizedCategory = NormalizeKey(category);

        return Task.FromResult(_tags.FirstOrDefault(tag =>
            tag.NormalizedName == normalizedName &&
            tag.NormalizedCategory == normalizedCategory));
    }

    public Task<IReadOnlyList<Tag>> ListAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Tag>>(
            _tags
                .OrderBy(tag => tag.Category)
                .ThenBy(tag => tag.Name)
                .ToArray());
    }

    public void Remove(Tag tag)
    {
        _tags.Remove(tag);
    }

    private static string NormalizeKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
    }
}

internal sealed class InMemoryContactRepository : IContactRepository
{
    private readonly List<Contact> _contacts = [];

    public IReadOnlyList<Contact> Contacts => _contacts;

    public Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        _contacts.Add(contact);
        return Task.CompletedTask;
    }

    public Task<Contact?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_contacts.FirstOrDefault(contact => contact.Id == id));
    }

    public Task<Contact?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_contacts.FirstOrDefault(contact =>
            contact.NormalizedEmail == normalizedEmail));
    }

    public Task<IReadOnlyList<Contact>> ListAsync(
        ContactFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Contact> query = _contacts;

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(contact =>
                contact.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                contact.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (contact.Role is not null &&
                    contact.Role.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        if (filter.TagId is not null)
        {
            query = query.Where(contact => contact.Tags.Any(tag => tag.TagId == filter.TagId));
        }

        if (filter.Status is not null)
        {
            query = query.Where(contact => contact.Status == filter.Status);
        }

        if (filter.DoNotContact is not null)
        {
            query = query.Where(contact => contact.DoNotContact == filter.DoNotContact);
        }

        if (filter.OrganizationId is not null)
        {
            query = query.Where(contact => contact.OrganizationId == filter.OrganizationId);
        }

        if (filter.LastContactedFrom is not null)
        {
            query = query.Where(contact => contact.LastContactedAt >= filter.LastContactedFrom);
        }

        if (filter.LastContactedTo is not null)
        {
            query = query.Where(contact => contact.LastContactedAt <= filter.LastContactedTo);
        }

        return Task.FromResult<IReadOnlyList<Contact>>(
            query.OrderBy(contact => contact.DisplayName).ToArray());
    }

    public void Remove(Contact contact)
    {
        _contacts.Remove(contact);
    }
}

internal sealed class InMemoryContactLookupService(
    InMemoryOrganizationRepository organizationRepository,
    InMemoryTagRepository tagRepository)
    : IContactLookupService
{
    public Task<ContactDto> MapAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Map(contact));
    }

    public Task<IReadOnlyList<ContactDto>> MapAsync(
        IReadOnlyList<Contact> contacts,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ContactDto>>(contacts.Select(Map).ToArray());
    }

    private ContactDto Map(Contact contact)
    {
        var organizationName = contact.OrganizationId is Guid organizationId
            ? organizationRepository.Organizations.FirstOrDefault(organization => organization.Id == organizationId)?.Name
            : null;

        var tags = contact.Tags
            .Select(contactTag => tagRepository.Tags.FirstOrDefault(tag => tag.Id == contactTag.TagId))
            .Where(tag => tag is not null)
            .Select(tag => new ContactTagDto(tag!.Id, tag.Name, tag.Category))
            .OrderBy(tag => tag.Category)
            .ThenBy(tag => tag.Name)
            .ToArray();

        return new ContactDto(
            contact.Id,
            contact.OrganizationId,
            organizationName,
            contact.DisplayName,
            contact.Email,
            contact.Phone,
            contact.Role,
            contact.Source,
            contact.Status,
            contact.DoNotContact,
            contact.LastContactedAt,
            contact.CreatedAt,
            contact.UpdatedAt,
            tags);
    }
}

internal sealed class InMemoryContactActivityRepository : IContactActivityRepository
{
    private readonly List<ContactActivity> _activities = [];

    public IReadOnlyList<ContactActivity> Activities => _activities;

    public Task AddAsync(ContactActivity activity, CancellationToken cancellationToken = default)
    {
        _activities.Add(activity);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ContactActivity>> ListByContactIdAsync(
        Guid contactId,
        CancellationToken cancellationToken = default)
    {
        var activities = _activities
            .Where(activity => activity.ContactId == contactId)
            .OrderByDescending(activity => activity.OccurredAt)
            .ToArray();

        return Task.FromResult<IReadOnlyList<ContactActivity>>(activities);
    }
}

internal sealed class InMemorySenderProfileRepository : ISenderProfileRepository
{
    private readonly List<SenderProfile> _senderProfiles = [];

    public IReadOnlyList<SenderProfile> SenderProfiles => _senderProfiles;

    public Task AddAsync(SenderProfile senderProfile, CancellationToken cancellationToken = default)
    {
        _senderProfiles.Add(senderProfile);
        return Task.CompletedTask;
    }

    public Task<SenderProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_senderProfiles.FirstOrDefault(senderProfile => senderProfile.Id == id));
    }

    public Task<SenderProfile?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_senderProfiles.FirstOrDefault(senderProfile =>
            senderProfile.IsActive && senderProfile.IsDefault));
    }

    public Task<IReadOnlyList<SenderProfile>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<SenderProfile> query = _senderProfiles;

        if (activeOnly is not null)
        {
            query = query.Where(senderProfile => senderProfile.IsActive == activeOnly);
        }

        return Task.FromResult<IReadOnlyList<SenderProfile>>(
            query
                .OrderByDescending(senderProfile => senderProfile.IsDefault)
                .ThenBy(senderProfile => senderProfile.Name)
                .ToArray());
    }

    public void Remove(SenderProfile senderProfile)
    {
        _senderProfiles.Remove(senderProfile);
    }
}

internal sealed class InMemoryEmailTemplateRepository : IEmailTemplateRepository
{
    private readonly List<EmailTemplate> _emailTemplates = [];

    public IReadOnlyList<EmailTemplate> EmailTemplates => _emailTemplates;

    public Task AddAsync(EmailTemplate emailTemplate, CancellationToken cancellationToken = default)
    {
        _emailTemplates.Add(emailTemplate);
        return Task.CompletedTask;
    }

    public Task<EmailTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_emailTemplates.FirstOrDefault(emailTemplate => emailTemplate.Id == id));
    }

    public Task<IReadOnlyList<EmailTemplate>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<EmailTemplate> query = _emailTemplates;

        if (activeOnly is not null)
        {
            query = query.Where(emailTemplate => emailTemplate.IsActive == activeOnly);
        }

        return Task.FromResult<IReadOnlyList<EmailTemplate>>(
            query.OrderBy(emailTemplate => emailTemplate.Name).ToArray());
    }

    public void Remove(EmailTemplate emailTemplate)
    {
        _emailTemplates.Remove(emailTemplate);
    }
}

internal sealed class InMemoryAttachmentAssetRepository : IAttachmentAssetRepository
{
    private readonly List<AttachmentAsset> _attachmentAssets = [];

    public IReadOnlyList<AttachmentAsset> AttachmentAssets => _attachmentAssets;

    public Task AddAsync(AttachmentAsset attachmentAsset, CancellationToken cancellationToken = default)
    {
        _attachmentAssets.Add(attachmentAsset);
        return Task.CompletedTask;
    }

    public Task<AttachmentAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_attachmentAssets.FirstOrDefault(attachmentAsset => attachmentAsset.Id == id));
    }

    public Task<IReadOnlyList<AttachmentAsset>> ListAsync(
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<AttachmentAsset> query = _attachmentAssets;

        if (activeOnly is not null)
        {
            query = query.Where(attachmentAsset => attachmentAsset.IsActive == activeOnly);
        }

        return Task.FromResult<IReadOnlyList<AttachmentAsset>>(
            query.OrderBy(attachmentAsset => attachmentAsset.Name).ToArray());
    }
}

internal sealed class InMemoryAttachmentFileStorage : IAttachmentFileStorage
{
    private readonly Func<AttachmentFileSaveRequest, StoredAttachmentFile> _saveHandler;

    public InMemoryAttachmentFileStorage(Func<AttachmentFileSaveRequest, StoredAttachmentFile>? saveHandler = null)
    {
        _saveHandler = saveHandler ?? (request => new StoredAttachmentFile(
            request.FileName,
            request.ContentType,
            $"storage/{request.FileName}",
            request.SizeBytes));
    }

    public Task<StoredAttachmentFile> SaveAsync(
        AttachmentFileSaveRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_saveHandler(request));
    }
}

internal sealed class InMemoryEmailDraftRepository : IEmailDraftRepository
{
    private readonly List<EmailDraft> _drafts = [];

    public IReadOnlyList<EmailDraft> Drafts => _drafts;

    public Task AddRangeAsync(IReadOnlyList<EmailDraft> drafts, CancellationToken cancellationToken = default)
    {
        _drafts.AddRange(drafts);
        return Task.CompletedTask;
    }

    public Task<EmailDraft?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_drafts.FirstOrDefault(draft => draft.Id == id));
    }

    public Task<IReadOnlyList<EmailDraft>> ListAsync(
        EmailDraftFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<EmailDraft> query = _drafts;

        if (filter.Status is not null)
        {
            query = query.Where(draft => draft.Status == filter.Status);
        }

        if (filter.ContactId is not null)
        {
            query = query.Where(draft => draft.ContactId == filter.ContactId);
        }

        return Task.FromResult<IReadOnlyList<EmailDraft>>(
            query.OrderByDescending(draft => draft.CreatedAt).ToArray());
    }
}

internal sealed class InMemoryEmailMessageRepository : IEmailMessageRepository
{
    private readonly List<EmailMessage> _emailMessages = [];

    public IReadOnlyList<EmailMessage> EmailMessages => _emailMessages;

    public Task AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        _emailMessages.Add(emailMessage);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsEquivalentSentEmailAsync(
        Guid contactId,
        string subject,
        DateTimeOffset since,
        CancellationToken cancellationToken = default)
    {
        var exists = _emailMessages.Any(emailMessage =>
            emailMessage.ContactId == contactId &&
            emailMessage.Status == EmailMessageStatus.Sent &&
            emailMessage.Subject == subject &&
            emailMessage.CreatedAt >= since);

        return Task.FromResult(exists);
    }
}

internal sealed class InMemoryFollowUpTaskRepository : IFollowUpTaskRepository
{
    private readonly List<FollowUpTask> _tasks = [];

    public IReadOnlyList<FollowUpTask> Tasks => _tasks;

    public Task AddAsync(FollowUpTask task, CancellationToken cancellationToken = default)
    {
        _tasks.Add(task);
        return Task.CompletedTask;
    }

    public Task<FollowUpTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tasks.FirstOrDefault(task => task.Id == id));
    }

    public Task<IReadOnlyList<FollowUpTask>> ListAsync(
        FollowUpTaskFilterRequest filter,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<FollowUpTask> query = _tasks;

        if (filter.ContactId is not null)
        {
            query = query.Where(task => task.ContactId == filter.ContactId);
        }

        if (filter.IsCompleted is not null)
        {
            query = query.Where(task => task.IsCompleted == filter.IsCompleted);
        }

        if (filter.DueFrom is not null)
        {
            query = query.Where(task => task.DueAt >= filter.DueFrom);
        }

        if (filter.DueTo is not null)
        {
            query = query.Where(task => task.DueAt <= filter.DueTo);
        }

        query = query.OrderBy(task => task.DueAt);

        if (filter.Limit is int limit && limit > 0)
        {
            query = query.Take(limit);
        }

        return Task.FromResult<IReadOnlyList<FollowUpTask>>(query.ToArray());
    }
}

internal sealed class InMemoryEmailSender(
    Func<SendEmailCommand, EmailSendResult>? sendHandler = null) : IEmailSender
{
    private readonly Func<SendEmailCommand, EmailSendResult> _sendHandler =
        sendHandler ?? (_ => new EmailSendResult(true, "Fake", $"fake-{Guid.NewGuid():N}", null));

    public List<SendEmailCommand> SentCommands { get; } = [];

    public Task<EmailSendResult> SendAsync(
        SendEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        SentCommands.Add(command);
        return Task.FromResult(_sendHandler(command));
    }
}

internal sealed class FixedEmailSendingPolicy(TimeSpan equivalentEmailWindow) : IEmailSendingPolicy
{
    public TimeSpan EquivalentEmailWindow { get; } = equivalentEmailWindow;
}

internal sealed class FixedFollowUpAutomationPolicy(
    bool autoCreateAfterSuccessfulSend,
    int autoCreateDueDays,
    FollowUpTaskType autoCreateType) : IFollowUpAutomationPolicy
{
    public bool AutoCreateAfterSuccessfulSend { get; } = autoCreateAfterSuccessfulSend;

    public int AutoCreateDueDays { get; } = autoCreateDueDays;

    public FollowUpTaskType AutoCreateType { get; } = autoCreateType;
}
