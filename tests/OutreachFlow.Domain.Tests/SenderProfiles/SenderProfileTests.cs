using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.SenderProfiles;

namespace OutreachFlow.Domain.Tests.SenderProfiles;

public sealed class SenderProfileTests
{
    [Fact]
    public void ShouldCreateActiveSenderProfile()
    {
        var profile = new SenderProfile(
            "Primary sender",
            "sender@example.com",
            "+34 600 000 000",
            "Northwind Studio",
            "https://example.com",
            "<p>Best regards</p>",
            SenderSignatureFormat.Html,
            isDefault: true);

        profile.Id.Should().NotBeEmpty();
        profile.Name.Should().Be("Primary sender");
        profile.Email.Should().Be("sender@example.com");
        profile.NormalizedEmail.Should().Be("SENDER@EXAMPLE.COM");
        profile.IsActive.Should().BeTrue();
        profile.IsDefault.Should().BeTrue();
        profile.SignatureFormat.Should().Be(SenderSignatureFormat.Html);
    }

    [Fact]
    public void ShouldRejectSenderProfileWithoutEmail()
    {
        var act = () => new SenderProfile("Primary sender", " ");

        act.Should().Throw<DomainException>()
            .WithMessage("Email is required.");
    }

    [Fact]
    public void ShouldRejectSignatureContentWithoutFormat()
    {
        var act = () => new SenderProfile(
            "Primary sender",
            "sender@example.com",
            signature: "<p>Best regards</p>",
            signatureFormat: null);

        act.Should().Throw<DomainException>()
            .WithMessage("Signature format is required when signature content is provided.");
    }

    [Fact]
    public void ShouldRejectRtfSignatureWithInvalidPayload()
    {
        var act = () => new SenderProfile(
            "Primary sender",
            "sender@example.com",
            signature: "Best regards",
            signatureFormat: SenderSignatureFormat.Rtf);

        act.Should().Throw<DomainException>()
            .WithMessage("RTF signature content must start with '{\\rtf'.");
    }

    [Fact]
    public void ShouldClearDefaultWhenDeactivated()
    {
        var profile = new SenderProfile("Primary sender", "sender@example.com", isDefault: true);

        profile.Deactivate(DateTimeOffset.UtcNow);

        profile.IsActive.Should().BeFalse();
        profile.IsDefault.Should().BeFalse();
    }

    [Fact]
    public void ShouldRejectMarkingInactiveSenderProfileAsDefault()
    {
        var profile = new SenderProfile("Primary sender", "sender@example.com");
        profile.Deactivate(DateTimeOffset.UtcNow);

        var act = () => profile.MarkDefault(DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Inactive sender profiles cannot be default.");
    }
}
