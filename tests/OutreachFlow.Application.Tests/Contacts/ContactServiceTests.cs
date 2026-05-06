using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tags;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.Tests.Contacts;

public sealed class ContactServiceTests
{
    [Fact]
    public async Task ShouldCreateContactWithOrganizationWhenRequestIsValid()
    {
        var fixture = new ContactServiceFixture();
        var organization = await fixture.OrganizationService.CreateAsync(new CreateOrganizationRequest(
            "Northwind Studio",
            null,
            null,
            null,
            null,
            null,
            null));

        var contact = await fixture.ContactService.CreateAsync(new CreateContactRequest(
            organization.Id,
            "Alex Morgan",
            "alex@example.com",
            "+34 600 000 000",
            "Operations Manager",
            "Manual",
            ContactStatus.New,
            false));

        contact.Id.Should().NotBeEmpty();
        contact.OrganizationId.Should().Be(organization.Id);
        contact.OrganizationName.Should().Be("Northwind Studio");
        contact.DisplayName.Should().Be("Alex Morgan");
        contact.Email.Should().Be("alex@example.com");
        fixture.ContactRepository.Contacts.Should().ContainSingle();
        fixture.ContactActivityRepository.Activities.Should().ContainSingle(activity =>
            activity.Type == ContactActivityType.ContactCreated &&
            activity.ContactId == contact.Id);
    }

    [Fact]
    public async Task ShouldRejectDuplicateContactEmail()
    {
        var fixture = new ContactServiceFixture();
        await fixture.ContactService.CreateAsync(CreateContactRequest("alex@example.com"));

        var act = () => fixture.ContactService.CreateAsync(CreateContactRequest(" ALEX@example.com "));

        await act.Should().ThrowAsync<ApplicationConflictException>()
            .WithMessage("A contact with this email already exists.");
    }

    [Fact]
    public async Task ShouldUpdateContactFieldsAndStatus()
    {
        var fixture = new ContactServiceFixture();
        var contact = await fixture.ContactService.CreateAsync(CreateContactRequest("alex@example.com"));

        var updated = await fixture.ContactService.UpdateAsync(contact.Id, new UpdateContactRequest(
            null,
            "Alex Morgan",
            "alex.morgan@example.com",
            null,
            "Founder",
            "Referral",
            ContactStatus.Contacted,
            false));

        updated.Email.Should().Be("alex.morgan@example.com");
        updated.Role.Should().Be("Founder");
        updated.Source.Should().Be("Referral");
        updated.Status.Should().Be(ContactStatus.Contacted);
        fixture.ContactActivityRepository.Activities.Should().Contain(activity =>
            activity.Type == ContactActivityType.ContactUpdated &&
            activity.ContactId == contact.Id);
        fixture.ContactActivityRepository.Activities.Should().Contain(activity =>
            activity.Type == ContactActivityType.StatusChanged &&
            activity.ContactId == contact.Id);
    }

    [Fact]
    public async Task ShouldFilterContactsByTagStatusDoNotContactAndOrganization()
    {
        var fixture = new ContactServiceFixture();
        var organization = await fixture.OrganizationService.CreateAsync(new CreateOrganizationRequest(
            "Northwind Studio",
            null,
            null,
            null,
            null,
            null,
            null));
        var priorityTag = await fixture.TagService.CreateAsync(new CreateTagRequest("Priority", null));
        var matchingContact = await fixture.ContactService.CreateAsync(new CreateContactRequest(
            organization.Id,
            "Alex Morgan",
            "alex@example.com",
            null,
            null,
            null,
            ContactStatus.Contacted,
            false));
        await fixture.ContactService.CreateAsync(new CreateContactRequest(
            organization.Id,
            "Sam Taylor",
            "sam@example.com",
            null,
            null,
            null,
            ContactStatus.New,
            false));
        await fixture.ContactService.AssignTagAsync(matchingContact.Id, priorityTag.Id);

        var contacts = await fixture.ContactService.ListAsync(new ContactFilterRequest(
            "Alex",
            priorityTag.Id,
            ContactStatus.Contacted,
            false,
            organization.Id,
            null,
            null));

        contacts.Should().ContainSingle()
            .Which.Id.Should().Be(matchingContact.Id);
    }

    [Fact]
    public async Task ShouldAssignAndRemoveContactTag()
    {
        var fixture = new ContactServiceFixture();
        var contact = await fixture.ContactService.CreateAsync(CreateContactRequest("alex@example.com"));
        var tag = await fixture.TagService.CreateAsync(new CreateTagRequest("Priority", null));

        await fixture.ContactService.AssignTagAsync(contact.Id, tag.Id);
        await fixture.ContactService.AssignTagAsync(contact.Id, tag.Id);
        var taggedContact = await fixture.ContactService.GetByIdAsync(contact.Id);

        taggedContact!.Tags.Should().ContainSingle()
            .Which.Id.Should().Be(tag.Id);

        await fixture.ContactService.RemoveTagAsync(contact.Id, tag.Id);
        var untaggedContact = await fixture.ContactService.GetByIdAsync(contact.Id);

        untaggedContact!.Tags.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldRejectContactWithMissingOrganization()
    {
        var fixture = new ContactServiceFixture();

        var act = () => fixture.ContactService.CreateAsync(new CreateContactRequest(
            Guid.NewGuid(),
            "Alex Morgan",
            "alex@example.com",
            null,
            null,
            null,
            ContactStatus.New,
            false));

        await act.Should().ThrowAsync<ApplicationNotFoundException>()
            .WithMessage("Organization was not found.");
    }

    private static CreateContactRequest CreateContactRequest(string email)
    {
        return new CreateContactRequest(
            null,
            "Alex Morgan",
            email,
            null,
            null,
            null,
            ContactStatus.New,
            false);
    }

    private sealed class ContactServiceFixture
    {
        public ContactServiceFixture()
        {
            UnitOfWork = new InMemoryUnitOfWork();
            OrganizationRepository = new InMemoryOrganizationRepository();
            TagRepository = new InMemoryTagRepository();
            ContactRepository = new InMemoryContactRepository();
            ContactActivityRepository = new InMemoryContactActivityRepository();
            ContactActivityService = new ContactActivityService(ContactRepository, ContactActivityRepository);
            ContactLookupService = new InMemoryContactLookupService(
                OrganizationRepository,
                TagRepository);

            OrganizationService = new OrganizationService(OrganizationRepository, UnitOfWork);
            TagService = new TagService(TagRepository, UnitOfWork);
            ContactService = new ContactService(
                ContactRepository,
                OrganizationRepository,
                TagRepository,
                ContactActivityService,
                ContactLookupService,
                UnitOfWork);
        }

        public InMemoryUnitOfWork UnitOfWork { get; }

        public InMemoryOrganizationRepository OrganizationRepository { get; }

        public InMemoryTagRepository TagRepository { get; }

        public InMemoryContactRepository ContactRepository { get; }

        public InMemoryContactActivityRepository ContactActivityRepository { get; }

        public InMemoryContactLookupService ContactLookupService { get; }

        public ContactActivityService ContactActivityService { get; }

        public OrganizationService OrganizationService { get; }

        public TagService TagService { get; }

        public ContactService ContactService { get; }
    }
}
