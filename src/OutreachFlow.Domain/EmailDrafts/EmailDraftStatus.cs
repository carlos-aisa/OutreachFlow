namespace OutreachFlow.Domain.EmailDrafts;

public enum EmailDraftStatus
{
    Draft = 1,
    NeedsReview = 2,
    Approved = 3,
    Queued = 4,
    Sent = 5,
    Failed = 6,
    Cancelled = 7
}
