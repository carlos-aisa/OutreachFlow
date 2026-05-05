using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.SenderProfiles;
using OutreachFlow.Application.Tests.Support;

namespace OutreachFlow.Application.Tests.SenderProfiles;

public sealed class SenderProfileServiceTests
{
    [Fact]
    public async Task ShouldClearExistingDefaultWhenNewDefaultProfileIsCreated()
    {
        var repository = new InMemorySenderProfileRepository();
        var service = new SenderProfileService(repository, new InMemoryUnitOfWork());
        var firstProfile = await service.CreateAsync(CreateRequest("Primary", "primary@example.com", true));

        var secondProfile = await service.CreateAsync(CreateRequest("Secondary", "secondary@example.com", true));

        var profiles = await service.ListAsync(activeOnly: null);
        profiles.Single(profile => profile.Id == firstProfile.Id).IsDefault.Should().BeFalse();
        profiles.Single(profile => profile.Id == secondProfile.Id).IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnDefaultSenderProfile()
    {
        var service = new SenderProfileService(
            new InMemorySenderProfileRepository(),
            new InMemoryUnitOfWork());
        var senderProfile = await service.CreateAsync(CreateRequest("Primary", "primary@example.com", true));

        var defaultProfile = await service.GetDefaultAsync();

        defaultProfile!.Id.Should().Be(senderProfile.Id);
    }

    [Fact]
    public async Task ShouldDeactivateSenderProfileAndClearDefault()
    {
        var service = new SenderProfileService(
            new InMemorySenderProfileRepository(),
            new InMemoryUnitOfWork());
        var senderProfile = await service.CreateAsync(CreateRequest("Primary", "primary@example.com", true));

        await service.DeactivateAsync(senderProfile.Id);

        var deactivatedProfile = await service.GetByIdAsync(senderProfile.Id);
        deactivatedProfile!.IsActive.Should().BeFalse();
        deactivatedProfile.IsDefault.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldRejectSenderProfileWithoutEmail()
    {
        var service = new SenderProfileService(
            new InMemorySenderProfileRepository(),
            new InMemoryUnitOfWork());

        var act = () => service.CreateAsync(CreateRequest("Primary", " ", false));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Email is required.");
    }

    private static CreateSenderProfileRequest CreateRequest(
        string name,
        string email,
        bool isDefault)
    {
        return new CreateSenderProfileRequest(
            name,
            email,
            null,
            "Northwind Studio",
            "https://example.com",
            "Best regards",
            isDefault);
    }
}
