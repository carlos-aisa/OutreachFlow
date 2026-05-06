using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.FollowUps;

public sealed class FollowUpTask
{
    private FollowUpTask()
    {
        Notes = string.Empty;
    }

    public FollowUpTask(
        Guid contactId,
        Guid? organizationId,
        DateTimeOffset dueAt,
        FollowUpTaskType type,
        string? notes = null,
        DateTimeOffset? createdAt = null)
    {
        if (contactId == Guid.Empty)
        {
            throw new DomainException("Contact id is required.");
        }

        Id = Guid.NewGuid();
        ContactId = contactId;
        OrganizationId = organizationId;
        DueAt = dueAt;
        Type = type;
        Notes = NormalizeOptional(notes);
        IsCompleted = false;
        CompletedAt = null;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public Guid ContactId { get; private set; }

    public Guid? OrganizationId { get; private set; }

    public DateTimeOffset DueAt { get; private set; }

    public FollowUpTaskType Type { get; private set; }

    public string? Notes { get; private set; }

    public bool IsCompleted { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public void Update(
        DateTimeOffset dueAt,
        FollowUpTaskType type,
        string? notes,
        DateTimeOffset updatedAt)
    {
        DueAt = dueAt;
        Type = type;
        Notes = NormalizeOptional(notes);
        UpdatedAt = updatedAt;
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (IsCompleted)
        {
            throw new DomainException("Follow-up task is already completed.");
        }

        IsCompleted = true;
        CompletedAt = completedAt;
        UpdatedAt = completedAt;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
