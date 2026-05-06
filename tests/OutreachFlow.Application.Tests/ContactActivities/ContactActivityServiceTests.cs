using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.Tests.ContactActivities;

public sealed class ContactActivityServiceTests
{
    [Fact]
    public async Task ShouldRecordAndListContactActivitiesInDescendingOrder()
    {
        var contactRepository = new InMemoryContactRepository();
        var activityRepository = new InMemoryContactActivityRepository();
        var service = new ContactActivityService(contactRepository, activityRepository);
        var contact = new Contact("Alex Morgan", "alex@example.com");
        await contactRepository.AddAsync(contact);

        var firstOccurredAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var secondOccurredAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        await service.RecordAsync(new CreateContactActivityRequest(
            contact.Id,
            null,
            ContactActivityType.ContactCreated,
            "Contact created",
            null,
            null,
            firstOccurredAt));
        await service.RecordAsync(new CreateContactActivityRequest(
            contact.Id,
            null,
            ContactActivityType.ContactUpdated,
            "Contact updated",
            null,
            null,
            secondOccurredAt));

        var activities = await service.ListByContactIdAsync(contact.Id);

        activities.Should().HaveCount(2);
        activities[0].Type.Should().Be(ContactActivityType.ContactUpdated);
        activities[1].Type.Should().Be(ContactActivityType.ContactCreated);
    }

    [Fact]
    public async Task ShouldRejectActivityListingForUnknownContact()
    {
        var service = new ContactActivityService(
            new InMemoryContactRepository(),
            new InMemoryContactActivityRepository());

        var act = () => service.ListByContactIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<ApplicationNotFoundException>()
            .WithMessage("Contact was not found.");
    }
}
