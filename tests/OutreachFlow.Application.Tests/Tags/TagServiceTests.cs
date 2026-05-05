using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Tags;
using OutreachFlow.Application.Tests.Support;

namespace OutreachFlow.Application.Tests.Tags;

public sealed class TagServiceTests
{
    [Fact]
    public async Task ShouldCreateTagWhenRequestIsValid()
    {
        var tagRepository = new InMemoryTagRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TagService(tagRepository, unitOfWork);

        var tag = await service.CreateAsync(new CreateTagRequest("Priority", "Workflow"));

        tag.Id.Should().NotBeEmpty();
        tag.Name.Should().Be("Priority");
        tag.Category.Should().Be("Workflow");
        tagRepository.Tags.Should().ContainSingle();
        unitOfWork.SaveChangesCount.Should().Be(1);
    }

    [Fact]
    public async Task ShouldRejectDuplicateTagInSameCategory()
    {
        var tagRepository = new InMemoryTagRepository();
        var service = new TagService(tagRepository, new InMemoryUnitOfWork());
        await service.CreateAsync(new CreateTagRequest("Priority", "Workflow"));

        var act = () => service.CreateAsync(new CreateTagRequest(" priority ", " workflow "));

        await act.Should().ThrowAsync<ApplicationConflictException>()
            .WithMessage("Tag already exists in this category.");
    }

    [Fact]
    public async Task ShouldUpdateTagWhenItExists()
    {
        var tagRepository = new InMemoryTagRepository();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new TagService(tagRepository, unitOfWork);
        var tag = await service.CreateAsync(new CreateTagRequest("Lead", null));

        var updated = await service.UpdateAsync(tag.Id, new UpdateTagRequest("Partner", "Relationship"));

        updated.Name.Should().Be("Partner");
        updated.Category.Should().Be("Relationship");
        unitOfWork.SaveChangesCount.Should().Be(2);
    }
}
