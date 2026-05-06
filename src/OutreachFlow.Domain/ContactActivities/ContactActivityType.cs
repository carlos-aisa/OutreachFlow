namespace OutreachFlow.Domain.ContactActivities;

public enum ContactActivityType
{
    ContactCreated = 1,
    ContactUpdated = 2,
    EmailDraftCreated = 3,
    EmailSent = 4,
    EmailFailed = 5,
    ReplyReceived = 6,
    NoteAdded = 7,
    StatusChanged = 8,
    FollowUpCreated = 9,
    FollowUpCompleted = 10
}
