using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.Campaigns;

public sealed class Campaign
{
    private Campaign() { Name = string.Empty; }
    public Campaign(string name, DateTimeOffset? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Campaign name is required.");
        Id = Guid.NewGuid(); Name = name.Trim(); CreatedAt = createdAt ?? DateTimeOffset.UtcNow; UpdatedAt = CreatedAt;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Subject { get; private set; }
    public string? Body { get; private set; }
    public Guid? SenderProfileId { get; private set; }
    public DateTimeOffset? FollowUpDueAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public void UpdateDetails(string name, string? subject, string? body, Guid? senderProfileId, DateTimeOffset? followUpDueAt)
    { if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Campaign name is required."); Name = name.Trim(); Subject = Normalize(subject); Body = Normalize(body); SenderProfileId = senderProfileId; FollowUpDueAt = followUpDueAt; UpdatedAt = DateTimeOffset.UtcNow; }
    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
