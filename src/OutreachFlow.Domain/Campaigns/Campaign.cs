using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.FollowUps;

namespace OutreachFlow.Domain.Campaigns;

public sealed class Campaign
{
    private readonly List<CampaignAudienceGroup> _audienceGroups = [];

    private Campaign()
    {
        Name = string.Empty;
    }

    public Campaign(
        string name,
        string? description,
        Guid emailTemplateId,
        IReadOnlyCollection<Guid> audienceGroupIds,
        bool followUpEnabled = false,
        int followUpDueDays = 7,
        FollowUpTaskType followUpType = FollowUpTaskType.Email,
        DateTimeOffset? createdAt = null)
    {
        Id = Guid.NewGuid();
        Name = RequireText(name, "Campaign name is required.");
        Description = NormalizeOptional(description);
        EmailTemplateId = RequireId(emailTemplateId, "Campaign message is required.");
        Status = CampaignStatus.Open;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;

        var distinctGroupIds = (audienceGroupIds ?? []).Distinct().ToArray();

        if (distinctGroupIds.Length == 0)
        {
            throw new DomainException("Campaign requires at least one audience group.");
        }

        foreach (var groupId in distinctGroupIds)
        {
            _audienceGroups.Add(new CampaignAudienceGroup(Id, groupId));
        }

        SetFollowUpSettings(followUpEnabled, followUpDueDays, followUpType);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public Guid EmailTemplateId { get; private set; }

    public CampaignStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool FollowUpEnabled { get; private set; }

    public int FollowUpDueDays { get; private set; }

    public FollowUpTaskType FollowUpType { get; private set; }

    public IReadOnlyCollection<CampaignAudienceGroup> AudienceGroups => _audienceGroups.AsReadOnly();

    public void ConfigureFollowUp(bool enabled, int dueDays, FollowUpTaskType type, DateTimeOffset? updatedAt = null)
    {
        SetFollowUpSettings(enabled, dueDays, type);
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public void Rename(string name, string? description, DateTimeOffset? updatedAt = null)
    {
        Name = RequireText(name, "Campaign name is required.");
        Description = NormalizeOptional(description);
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public void ChangeMessage(Guid emailTemplateId, DateTimeOffset? updatedAt = null)
    {
        EnsureOpen();
        EmailTemplateId = RequireId(emailTemplateId, "Campaign message is required.");
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public bool AddAudienceGroup(Guid contactGroupId, DateTimeOffset? updatedAt = null)
    {
        EnsureOpen();

        if (_audienceGroups.Any(audienceGroup => audienceGroup.ContactGroupId == contactGroupId))
        {
            return false;
        }

        _audienceGroups.Add(new CampaignAudienceGroup(Id, contactGroupId));
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
        return true;
    }

    public bool RemoveAudienceGroup(Guid contactGroupId, DateTimeOffset? updatedAt = null)
    {
        EnsureOpen();

        var existing = _audienceGroups.FirstOrDefault(audienceGroup => audienceGroup.ContactGroupId == contactGroupId);

        if (existing is null)
        {
            return false;
        }

        if (_audienceGroups.Count == 1)
        {
            throw new DomainException("Campaign requires at least one audience group.");
        }

        _audienceGroups.Remove(existing);
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
        return true;
    }

    public void Close(DateTimeOffset? updatedAt = null)
    {
        if (Status == CampaignStatus.Closed)
        {
            throw new DomainException("Campaign is already closed.");
        }

        Status = CampaignStatus.Closed;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public void Reopen(DateTimeOffset? updatedAt = null)
    {
        if (Status == CampaignStatus.Open)
        {
            throw new DomainException("Campaign is already open.");
        }

        Status = CampaignStatus.Open;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    private void SetFollowUpSettings(bool enabled, int dueDays, FollowUpTaskType type)
    {
        if (enabled && dueDays <= 0)
        {
            throw new DomainException("Follow-up due days must be greater than zero when follow-up is enabled.");
        }

        FollowUpEnabled = enabled;
        FollowUpDueDays = dueDays;
        FollowUpType = type;
    }

    private void EnsureOpen()
    {
        if (Status != CampaignStatus.Open)
        {
            throw new DomainException("Only open campaigns can be changed.");
        }
    }

    private static string RequireText(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(message);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static Guid RequireId(Guid value, string message)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(message);
        }

        return value;
    }
}
