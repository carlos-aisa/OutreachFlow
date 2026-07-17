using OutreachFlow.Domain.Common;

namespace OutreachFlow.Domain.ContactGroups;

public sealed class ContactGroupCriterion
{
    private ContactGroupCriterion() { Value = string.Empty; NormalizedValue = string.Empty; }

    public ContactGroupCriterion(Guid contactGroupId, ContactGroupCriterionType type, string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Contact group criterion value is required.");
        ContactGroupId = contactGroupId;
        Type = type;
        Value = value.Trim();
        NormalizedValue = Value.ToUpperInvariant();
    }

    public Guid ContactGroupId { get; private set; }
    public ContactGroupCriterionType Type { get; private set; }
    public string Value { get; private set; }
    public string NormalizedValue { get; private set; }
}
