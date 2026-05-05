namespace OutreachFlow.Infrastructure.EmailSending;

public sealed class EmailSendingOptions
{
    public const string SectionName = "EmailSending";

    public string Provider { get; set; } = "Fake";

    public int EquivalentEmailWindowHours { get; set; } = 168;

    public string FakeFailureKeyword { get; set; } = "[fail-send]";
}
