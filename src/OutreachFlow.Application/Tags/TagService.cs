using OutreachFlow.Application.Common;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Application.Tags;

public sealed class TagService(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork)
    : ITagService
{
    public async Task<TagDto> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        var existingTag = await tagRepository.GetByNameAsync(request.Name, request.Category, cancellationToken);
        if (existingTag is not null)
        {
            throw new ApplicationConflictException("Tag already exists in this category.");
        }

        var tag = CreateTag(request);
        await tagRepository.AddAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(tag);
    }

    public async Task<TagDto> UpdateAsync(
        Guid id,
        UpdateTagRequest request,
        CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Tag was not found.");

        var existingTag = await tagRepository.GetByNameAsync(request.Name, request.Category, cancellationToken);
        if (existingTag is not null && existingTag.Id != id)
        {
            throw new ApplicationConflictException("Tag already exists in this category.");
        }

        try
        {
            tag.Update(request.Name, request.Category);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(tag);
    }

    public async Task<TagDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetByIdAsync(id, cancellationToken);
        return tag is null ? null : Map(tag);
    }

    public async Task<IReadOnlyList<TagDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var tags = await tagRepository.ListAsync(cancellationToken);
        return tags.Select(Map).ToArray();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new ApplicationNotFoundException("Tag was not found.");

        tagRepository.Remove(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static Tag CreateTag(CreateTagRequest request)
    {
        try
        {
            return new Tag(request.Name, request.Category);
        }
        catch (DomainException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }
    }

    private static TagDto Map(Tag tag)
    {
        return new TagDto(tag.Id, tag.Name, tag.Category, tag.CreatedAt);
    }
}
