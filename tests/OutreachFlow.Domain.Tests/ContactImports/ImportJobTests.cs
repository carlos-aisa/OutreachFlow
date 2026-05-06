using FluentAssertions;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.ContactImports;

namespace OutreachFlow.Domain.Tests.ContactImports;

public sealed class ImportJobTests
{
    [Fact]
    public void ShouldCreatePendingImportJob()
    {
        var job = new ImportJob(
            "contacts.csv",
            totalRows: 10,
            validRows: 8,
            duplicateRows: 1,
            invalidRows: 1);

        job.FileName.Should().Be("contacts.csv");
        job.Status.Should().Be(ImportJobStatus.Pending);
        job.CreatedCount.Should().Be(0);
        job.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void ShouldMarkImportJobCompleted()
    {
        var job = new ImportJob("contacts.csv", 10, 8, 1, 1);
        var completedAt = DateTimeOffset.UtcNow;

        job.MarkCompleted(
            createdCount: 8,
            duplicateRows: 1,
            invalidRows: 1,
            completedAt);

        job.Status.Should().Be(ImportJobStatus.Completed);
        job.CreatedCount.Should().Be(8);
        job.CompletedAt.Should().Be(completedAt);
        job.FailureReason.Should().BeNull();
    }

    [Fact]
    public void ShouldRejectBlankFailureReasonWhenMarkingFailed()
    {
        var job = new ImportJob("contacts.csv", 1, 1, 0, 0);

        var act = () => job.MarkFailed("   ", DateTimeOffset.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Import failure reason is required.");
    }
}
