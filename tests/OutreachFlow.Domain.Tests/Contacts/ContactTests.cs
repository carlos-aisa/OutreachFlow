using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Contacts;

namespace OutreachFlow.Domain.Tests.Contacts;

public sealed class ContactTests
{
    [Fact]
    public void ConstructorShouldCreateNewContactWhenInputIsValid()
    {
        var createdAt = new DateTimeOffset(2026, 5, 5, 0, 0, 0, TimeSpan.Zero);

        var contact = new Contact(
            "Ada Lovelace",
            "ada@example.com",
            phone: "555-0100",
            role: "Director",
            source: "Referral",
            createdAt: createdAt);

        contact.Id.Should().NotBeEmpty();
        contact.DisplayName.Should().Be("Ada Lovelace");
        contact.Email.Should().Be("ada@example.com");
        contact.NormalizedEmail.Should().Be("ADA@EXAMPLE.COM");
        contact.Status.Should().Be(ContactStatus.New);
        contact.DoNotContact.Should().BeFalse();
        contact.CreatedAt.Should().Be(createdAt);
        contact.UpdatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ConstructorShouldRejectMissingDisplayName()
    {
        var act = () => new Contact("", "ada@example.com");

        act.Should().Throw<DomainException>()
            .WithMessage("Contact display name is required.");
    }

    [Fact]
    public void ConstructorShouldRejectInvalidEmail()
    {
        var act = () => new Contact("Ada Lovelace", "not-an-email");

        act.Should().Throw<DomainException>()
            .WithMessage("Email format is invalid.");
    }

    [Fact]
    public void MarkDoNotContactShouldSetFlagAndStatus()
    {
        var contact = new Contact("Ada Lovelace", "ada@example.com");
        var updatedAt = new DateTimeOffset(2026, 5, 5, 1, 0, 0, TimeSpan.Zero);

        contact.MarkDoNotContact(updatedAt);

        contact.DoNotContact.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.DoNotContact);
        contact.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void ChangeStatusShouldSetDoNotContactWhenStatusRequiresIt()
    {
        var contact = new Contact("Ada Lovelace", "ada@example.com");

        contact.ChangeStatus(ContactStatus.DoNotContact, DateTimeOffset.UtcNow);

        contact.DoNotContact.Should().BeTrue();
        contact.Status.Should().Be(ContactStatus.DoNotContact);
    }

    [Fact]
    public void AssignTagShouldAddTagOnlyOnce()
    {
        var contact = new Contact("Ada Lovelace", "ada@example.com");
        var tagId = Guid.NewGuid();

        var firstResult = contact.AssignTag(tagId);
        var secondResult = contact.AssignTag(tagId);

        firstResult.Should().BeTrue();
        secondResult.Should().BeFalse();
        contact.Tags.Should().ContainSingle(tag => tag.TagId == tagId);
    }

    [Fact]
    public void RemoveTagShouldRemoveAssignedTag()
    {
        var contact = new Contact("Ada Lovelace", "ada@example.com");
        var tagId = Guid.NewGuid();
        contact.AssignTag(tagId);

        var result = contact.RemoveTag(tagId);

        result.Should().BeTrue();
        contact.Tags.Should().BeEmpty();
    }
}
