using FluentAssertions;
using OutreachFlow.Application.Attachments;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.Tests.Support;

namespace OutreachFlow.Application.Tests.Attachments;

public sealed class AttachmentAssetServiceTests
{
    [Fact]
    public async Task ShouldUploadAttachmentAndPersistMetadata()
    {
        var repository = new InMemoryAttachmentAssetRepository();
        var storage = new InMemoryAttachmentFileStorage();
        var service = new AttachmentAssetService(repository, storage, new InMemoryUnitOfWork());
        await using var content = new MemoryStream([1, 2, 3, 4]);

        var attachment = await service.UploadAsync(new UploadAttachmentAssetRequest(
            "Service brochure",
            "Overview",
            "brochure.pdf",
            "application/pdf",
            content,
            4));

        attachment.Name.Should().Be("Service brochure");
        attachment.FileName.Should().Be("brochure.pdf");
        attachment.ContentType.Should().Be("application/pdf");
        attachment.StoragePath.Should().Be("storage/brochure.pdf");
        attachment.SizeBytes.Should().Be(4);
        attachment.IsActive.Should().BeTrue();
        repository.AttachmentAssets.Should().ContainSingle();
    }

    [Fact]
    public async Task ShouldTranslateStorageValidationErrors()
    {
        var repository = new InMemoryAttachmentAssetRepository();
        var storage = new InMemoryAttachmentFileStorage(_ =>
            throw new InvalidOperationException("Attachment file name contains an unsafe path."));
        var service = new AttachmentAssetService(repository, storage, new InMemoryUnitOfWork());
        await using var content = new MemoryStream([1, 2, 3, 4]);

        var act = () => service.UploadAsync(new UploadAttachmentAssetRequest(
            "Service brochure",
            null,
            "../brochure.pdf",
            "application/pdf",
            content,
            4));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("Attachment file name contains an unsafe path.");
    }
}
