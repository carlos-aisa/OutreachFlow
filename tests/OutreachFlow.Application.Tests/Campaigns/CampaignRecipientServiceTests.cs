using FluentAssertions;
using OutreachFlow.Application.Campaigns;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.ContactGroups;
using OutreachFlow.Application.EmailDrafts;
using OutreachFlow.Application.Templates;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Campaigns;
using OutreachFlow.Domain.ContactGroups;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.EmailTemplates;
using OutreachFlow.Domain.FollowUps;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Application.Tests.Campaigns;

public sealed class CampaignRecipientServiceTests
{
    [Fact]
    public async Task ShouldDiscoverCandidatesFromAudienceGroupsExcludingExistingRecipients()
    {
        var (service, campaignRepository, contactGroupRepository, contactRepository, recipientRepository, templateRepository, _) = CreateService();
        var group = await AddGroupAsync(contactGroupRepository, "Prospects");
        var included = await AddContactAsync(contactRepository, "Alex Morgan", "alex@example.com");
        var alreadyRecipient = await AddContactAsync(contactRepository, "Jamie Smith", "jamie@example.com");
        await AddEvaluationContactsAsync(contactGroupRepository, included.Id, alreadyRecipient.Id);
        var template = await AddTemplateAsync(templateRepository);
        var campaign = new Campaign("Autumn outreach", null, template.Id, [group.Id]);
        await campaignRepository.AddAsync(campaign);
        await recipientRepository.AddAsync(new CampaignRecipient(campaign.Id, alreadyRecipient.Id, template.Id));

        var candidates = await service.DiscoverCandidatesAsync(campaign.Id);

        candidates.Should().ContainSingle(candidate => candidate.ContactId == included.Id);
    }

    [Fact]
    public async Task ShouldIncorporateEligibleContactAsIncorporated()
    {
        var (service, campaignRepository, _, contactRepository, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var contact = await AddContactAsync(contactRepository, "Alex Morgan", "alex@example.com");
        var campaign = new Campaign("Autumn outreach", null, template.Id, [Guid.NewGuid()]);
        await campaignRepository.AddAsync(campaign);

        var recipient = await service.IncorporateAsync(campaign.Id, contact.Id);

        recipient.Status.Should().Be(CampaignRecipientStatus.Incorporated);
        recipient.ContactDisplayName.Should().Be("Alex Morgan");
    }

    [Fact]
    public async Task ShouldRejectIncorporatingIntoClosedCampaign()
    {
        var (service, campaignRepository, _, contactRepository, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var contact = await AddContactAsync(contactRepository, "Alex Morgan", "alex@example.com");
        var campaign = new Campaign("Autumn outreach", null, template.Id, [Guid.NewGuid()]);
        campaign.Close();
        await campaignRepository.AddAsync(campaign);

        var act = () => service.IncorporateAsync(campaign.Id, contact.Id);

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Only open campaigns can incorporate recipients.");
    }

    [Fact]
    public async Task ShouldRejectDuplicateIncorporationForSameMessageVersion()
    {
        var (service, campaignRepository, _, contactRepository, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var contact = await AddContactAsync(contactRepository, "Alex Morgan", "alex@example.com");
        var campaign = new Campaign("Autumn outreach", null, template.Id, [Guid.NewGuid()]);
        await campaignRepository.AddAsync(campaign);
        await service.IncorporateAsync(campaign.Id, contact.Id);

        var act = () => service.IncorporateAsync(campaign.Id, contact.Id);

        await act.Should().ThrowAsync<ApplicationConflictException>();
    }

    [Fact]
    public async Task ShouldGenerateDraftsForIncorporatedRecipientsAndExcludeIneligibleOnes()
    {
        var (service, campaignRepository, _, contactRepository, recipientRepository, templateRepository, senderProfileRepository) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var senderProfile = new SenderProfile("Primary Sender", "sender@example.com");
        await senderProfileRepository.AddAsync(senderProfile);
        var eligible = await AddContactAsync(contactRepository, "Alex Morgan", "alex@example.com");
        var doNotContact = await AddContactAsync(contactRepository, "Jamie Smith", "jamie@example.com", doNotContact: true);
        var campaign = new Campaign("Autumn outreach", null, template.Id, [Guid.NewGuid()]);
        await campaignRepository.AddAsync(campaign);
        await service.IncorporateAsync(campaign.Id, eligible.Id);
        await service.IncorporateAsync(campaign.Id, doNotContact.Id);

        var result = await service.GenerateDraftsAsync(campaign.Id, new GenerateCampaignDraftsRequest(senderProfile.Id, []));

        result.RequestedRecipients.Should().Be(2);
        result.GeneratedDrafts.Should().Be(1);
        result.ExcludedRecipients.Should().Be(1);
        recipientRepository.Recipients.Single(recipient => recipient.ContactId == eligible.Id).Status
            .Should().Be(CampaignRecipientStatus.Drafted);
        recipientRepository.Recipients.Single(recipient => recipient.ContactId == doNotContact.Id).Status
            .Should().Be(CampaignRecipientStatus.Excluded);
    }

    [Fact]
    public async Task ShouldListRecipientsForCampaign()
    {
        var (service, campaignRepository, _, contactRepository, _, templateRepository, _) = CreateService();
        var template = await AddTemplateAsync(templateRepository);
        var contact = await AddContactAsync(contactRepository, "Alex Morgan", "alex@example.com");
        var campaign = new Campaign("Autumn outreach", null, template.Id, [Guid.NewGuid()]);
        await campaignRepository.AddAsync(campaign);
        await service.IncorporateAsync(campaign.Id, contact.Id);

        var recipients = await service.ListAsync(campaign.Id);

        recipients.Should().ContainSingle(recipient => recipient.ContactId == contact.Id);
    }

    private static (
        CampaignRecipientService Service,
        InMemoryCampaignRepository CampaignRepository,
        InMemoryContactGroupRepositoryForCandidates ContactGroupRepository,
        InMemoryContactRepository ContactRepository,
        InMemoryCampaignRecipientRepository RecipientRepository,
        InMemoryEmailTemplateRepository TemplateRepository,
        InMemorySenderProfileRepository SenderProfileRepository) CreateService()
    {
        var campaignRepository = new InMemoryCampaignRepository();
        var contactGroupRepository = new InMemoryContactGroupRepositoryForCandidates();
        var contactRepository = new InMemoryContactRepository();
        var recipientRepository = new InMemoryCampaignRecipientRepository();
        var templateRepository = new InMemoryEmailTemplateRepository();
        var senderProfileRepository = new InMemorySenderProfileRepository();
        var attachmentRepository = new InMemoryAttachmentAssetRepository();
        var draftRepository = new InMemoryEmailDraftRepository();
        var organizationRepository = new InMemoryOrganizationRepository();
        var emailMessageRepository = new InMemoryEmailMessageRepository();
        var followUpTaskRepository = new InMemoryFollowUpTaskRepository();
        var contactActivityRepository = new InMemoryContactActivityRepository();
        var contactActivityService = new ContactActivityService(contactRepository, contactActivityRepository);
        var unitOfWork = new InMemoryUnitOfWork();
        var emailSender = new InMemoryEmailSender();
        var policy = new FixedEmailSendingPolicy(TimeSpan.FromDays(7));
        var followUpPolicy = new FixedFollowUpAutomationPolicy(false, 7, FollowUpTaskType.Email);

        var emailDraftService = new EmailDraftService(
            contactRepository,
            organizationRepository,
            templateRepository,
            senderProfileRepository,
            attachmentRepository,
            draftRepository,
            emailMessageRepository,
            followUpTaskRepository,
            recipientRepository,
            campaignRepository,
            emailSender,
            policy,
            followUpPolicy,
            contactActivityService,
            new TemplateRenderer(),
            unitOfWork);

        var contactGroupService = new ContactGroupService(contactGroupRepository, unitOfWork);

        var service = new CampaignRecipientService(
            campaignRepository,
            recipientRepository,
            contactRepository,
            contactGroupService,
            emailDraftService,
            unitOfWork);

        return (service, campaignRepository, contactGroupRepository, contactRepository, recipientRepository, templateRepository, senderProfileRepository);
    }

    private static async Task<Contact> AddContactAsync(
        InMemoryContactRepository repository,
        string name,
        string email,
        bool doNotContact = false)
    {
        var contact = new Contact(name, email, doNotContact: doNotContact);
        await repository.AddAsync(contact);
        return contact;
    }

    private static async Task<EmailTemplate> AddTemplateAsync(InMemoryEmailTemplateRepository repository)
    {
        var template = new EmailTemplate("Intro", null, "Subject", "Body");
        await repository.AddAsync(template);
        return template;
    }

    private static async Task<ContactGroup> AddGroupAsync(InMemoryContactGroupRepositoryForCandidates repository, string name)
    {
        var group = new ContactGroup(name);
        await repository.AddAsync(group);
        return group;
    }

    private static Task AddEvaluationContactsAsync(
        InMemoryContactGroupRepositoryForCandidates repository,
        params Guid[] contactIds)
    {
        repository.Contacts = contactIds
            .Select(contactId => new ContactGroupEvaluationContact(contactId, null, null, null, new HashSet<Guid>()))
            .ToArray();
        return Task.CompletedTask;
    }

    private sealed class InMemoryContactGroupRepositoryForCandidates : IContactGroupRepository
    {
        private readonly List<ContactGroup> _groups = [];

        public IReadOnlyList<ContactGroupEvaluationContact> Contacts { get; set; } = [];

        public Task AddAsync(ContactGroup group, CancellationToken cancellationToken = default)
        {
            _groups.Add(group);
            return Task.CompletedTask;
        }

        public Task<ContactGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_groups.FirstOrDefault(group => group.Id == id));

        public Task<IReadOnlyList<ContactGroup>> ListAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ContactGroup>>(_groups);

        public Task<IReadOnlyList<ContactGroupCriterion>> ListCriteriaAsync(Guid contactGroupId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ContactGroupCriterion>>([]);

        public Task<IReadOnlyList<ContactGroupMembershipOverride>> ListOverridesAsync(Guid contactGroupId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ContactGroupMembershipOverride>>([]);

        public Task<IReadOnlyList<ContactGroupEvaluationContact>> ListEvaluationContactsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Contacts);

        public Task AddCriterionAsync(ContactGroupCriterion criterion, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task ReplaceCriteriaAsync(Guid contactGroupId, IReadOnlyList<ContactGroupCriterion> criteria, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task UpsertOverrideAsync(ContactGroupMembershipOverride membershipOverride, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task RemoveOverrideAsync(Guid contactGroupId, Guid contactId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void Remove(ContactGroup contactGroup) => _groups.Remove(contactGroup);
    }
}
