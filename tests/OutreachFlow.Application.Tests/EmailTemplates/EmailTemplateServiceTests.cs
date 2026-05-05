using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.EmailTemplates;
using OutreachFlow.Application.Tests.Support;

namespace OutreachFlow.Application.Tests.EmailTemplates;

public sealed class EmailTemplateServiceTests
{
    [Fact]
    public async Task ShouldCreateActiveEmailTemplate()
    {
        var repository = new InMemoryEmailTemplateRepository();
        var service = new EmailTemplateService(repository, new InMemoryUnitOfWork());

        var template = await service.CreateAsync(new CreateEmailTemplateRequest(
            "Intro",
            "Initial outreach",
            "Hello {{contact.displayName}}",
            "Hello {{contact.displayName}},\n\n{{sender.signature}}"));

        template.Id.Should().NotBeEmpty();
        template.IsActive.Should().BeTrue();
        repository.EmailTemplates.Should().ContainSingle();
    }

    [Fact]
    public async Task ShouldUpdateTemplateActiveState()
    {
        var service = new EmailTemplateService(
            new InMemoryEmailTemplateRepository(),
            new InMemoryUnitOfWork());
        var template = await service.CreateAsync(new CreateEmailTemplateRequest(
            "Intro",
            null,
            "Subject",
            "Body"));

        var updated = await service.UpdateAsync(template.Id, new UpdateEmailTemplateRequest(
            "Intro updated",
            "Updated",
            "Subject updated",
            "Body updated",
            false));

        updated.Name.Should().Be("Intro updated");
        updated.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldFilterActiveTemplates()
    {
        var service = new EmailTemplateService(
            new InMemoryEmailTemplateRepository(),
            new InMemoryUnitOfWork());
        var activeTemplate = await service.CreateAsync(new CreateEmailTemplateRequest(
            "Active",
            null,
            "Subject",
            "Body"));
        var inactiveTemplate = await service.CreateAsync(new CreateEmailTemplateRequest(
            "Inactive",
            null,
            "Subject",
            "Body"));
        await service.DeactivateAsync(inactiveTemplate.Id);

        var activeTemplates = await service.ListAsync(activeOnly: true);

        activeTemplates.Should().ContainSingle()
            .Which.Id.Should().Be(activeTemplate.Id);
    }

    [Fact]
    public async Task ShouldRejectTemplateWithoutBody()
    {
        var service = new EmailTemplateService(
            new InMemoryEmailTemplateRepository(),
            new InMemoryUnitOfWork());

        var act = () => service.CreateAsync(new CreateEmailTemplateRequest(
            "Intro",
            null,
            "Subject",
            " "));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Email template body is required.");
    }
}
