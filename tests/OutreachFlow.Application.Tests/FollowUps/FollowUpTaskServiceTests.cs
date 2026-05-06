using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.FollowUps;
using OutreachFlow.Application.Organizations;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Application.Tests.FollowUps;

public sealed class FollowUpTaskServiceTests
{
    [Fact]
    public async Task ShouldCreateFollowUpTaskAndRecordActivity()
    {
        var fixture = new Fixture();
        var contact = await fixture.ContactService.CreateAsync(new CreateContactRequest(
            null,
            "Alex Morgan",
            "alex@example.com",
            null,
            null,
            null,
            ContactStatus.New,
            false));

        var task = await fixture.FollowUpTaskService.CreateAsync(new CreateFollowUpTaskRequest(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(2),
            FollowUpTaskType.Email,
            "Check in next week."));

        task.ContactId.Should().Be(contact.Id);
        fixture.ContactActivityRepository.Activities.Should().Contain(activity =>
            activity.Type == ContactActivityType.FollowUpCreated &&
            activity.ContactId == contact.Id);
    }

    [Fact]
    public async Task ShouldCompleteFollowUpTaskAndRecordCompletionActivity()
    {
        var fixture = new Fixture();
        var contact = await fixture.ContactService.CreateAsync(new CreateContactRequest(
            null,
            "Alex Morgan",
            "alex@example.com",
            null,
            null,
            null,
            ContactStatus.New,
            false));
        var task = await fixture.FollowUpTaskService.CreateAsync(new CreateFollowUpTaskRequest(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(2),
            FollowUpTaskType.Call,
            null));

        var completed = await fixture.FollowUpTaskService.CompleteAsync(task.Id);

        completed.IsCompleted.Should().BeTrue();
        fixture.ContactActivityRepository.Activities.Should().Contain(activity =>
            activity.Type == ContactActivityType.FollowUpCompleted &&
            activity.ContactId == contact.Id);
    }

    [Fact]
    public async Task ShouldListPendingFollowUpTasksOrderedByDueAt()
    {
        var fixture = new Fixture();
        var contact = await fixture.ContactService.CreateAsync(new CreateContactRequest(
            null,
            "Alex Morgan",
            "alex@example.com",
            null,
            null,
            null,
            ContactStatus.New,
            false));
        await fixture.FollowUpTaskService.CreateAsync(new CreateFollowUpTaskRequest(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(3),
            FollowUpTaskType.Email,
            "Later"));
        await fixture.FollowUpTaskService.CreateAsync(new CreateFollowUpTaskRequest(
            contact.Id,
            null,
            DateTimeOffset.UtcNow.AddDays(1),
            FollowUpTaskType.Email,
            "Sooner"));

        var pending = await fixture.FollowUpTaskService.ListAsync(new FollowUpTaskFilterRequest(
            contact.Id,
            IsCompleted: false,
            DueFrom: null,
            DueTo: null));

        pending.Should().HaveCount(2);
        pending[0].Notes.Should().Be("Sooner");
        pending[1].Notes.Should().Be("Later");
    }

    [Fact]
    public async Task ShouldRejectFollowUpCreationForUnknownContact()
    {
        var fixture = new Fixture();

        var act = () => fixture.FollowUpTaskService.CreateAsync(new CreateFollowUpTaskRequest(
            Guid.NewGuid(),
            null,
            DateTimeOffset.UtcNow.AddDays(1),
            FollowUpTaskType.General,
            null));

        await act.Should().ThrowAsync<ApplicationNotFoundException>()
            .WithMessage("Contact was not found.");
    }

    private sealed class Fixture
    {
        public Fixture()
        {
            UnitOfWork = new InMemoryUnitOfWork();
            ContactRepository = new InMemoryContactRepository();
            OrganizationRepository = new InMemoryOrganizationRepository();
            TagRepository = new InMemoryTagRepository();
            ContactLookupService = new InMemoryContactLookupService(OrganizationRepository, TagRepository);
            ContactActivityRepository = new InMemoryContactActivityRepository();
            FollowUpTaskRepository = new InMemoryFollowUpTaskRepository();
            ContactActivityService = new ContactActivityService(ContactRepository, ContactActivityRepository);
            ContactService = new ContactService(
                ContactRepository,
                OrganizationRepository,
                TagRepository,
                ContactActivityService,
                ContactLookupService,
                UnitOfWork);
            FollowUpTaskService = new FollowUpTaskService(
                FollowUpTaskRepository,
                ContactRepository,
                OrganizationRepository,
                ContactActivityService,
                UnitOfWork);
        }

        public InMemoryUnitOfWork UnitOfWork { get; }

        public InMemoryContactRepository ContactRepository { get; }

        public InMemoryOrganizationRepository OrganizationRepository { get; }

        public InMemoryTagRepository TagRepository { get; }

        public InMemoryContactLookupService ContactLookupService { get; }

        public InMemoryContactActivityRepository ContactActivityRepository { get; }

        public InMemoryFollowUpTaskRepository FollowUpTaskRepository { get; }

        public ContactActivityService ContactActivityService { get; }

        public ContactService ContactService { get; }

        public FollowUpTaskService FollowUpTaskService { get; }
    }
}
