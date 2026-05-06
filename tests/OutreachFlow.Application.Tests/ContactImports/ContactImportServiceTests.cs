using FluentAssertions;
using OutreachFlow.Application.Common;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.ContactImports;
using OutreachFlow.Application.Tags;
using OutreachFlow.Application.Tests.Support;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.ContactActivities;

namespace OutreachFlow.Application.Tests.ContactImports;

public sealed class ContactImportServiceTests
{
    [Fact]
    public async Task ShouldPreviewValidRows()
    {
        var fixture = new ContactImportServiceFixture();

        var result = await fixture.Service.PreviewAsync(new ContactImportPreviewRequest(
            "contacts.csv",
            """
            displayName,email,phone,role,source
            Alex Morgan,alex@example.com,+34 600 000 000,Founder,Referral
            """));

        result.TotalRows.Should().Be(1);
        result.ValidRows.Should().Be(1);
        result.DuplicateRows.Should().Be(0);
        result.InvalidRows.Should().Be(0);
        result.Rows.Should().ContainSingle().Which.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldMarkMissingRequiredFieldsAsInvalid()
    {
        var fixture = new ContactImportServiceFixture();

        var result = await fixture.Service.PreviewAsync(new ContactImportPreviewRequest(
            "contacts.csv",
            """
            displayName,email
            ,missing-name@example.com
            Alex Morgan,
            """));

        result.TotalRows.Should().Be(2);
        result.ValidRows.Should().Be(0);
        result.InvalidRows.Should().Be(2);
        result.DuplicateRows.Should().Be(0);
        result.Rows.Should().OnlyContain(row => !row.IsValid && !row.IsDuplicate);
    }

    [Fact]
    public async Task ShouldMarkDuplicateRowsInFileAndDatabase()
    {
        var fixture = new ContactImportServiceFixture();
        await fixture.ContactRepository.AddAsync(new Contact(
            "Existing Contact",
            "existing@example.com",
            source: "Manual"));

        var result = await fixture.Service.PreviewAsync(new ContactImportPreviewRequest(
            "contacts.csv",
            """
            displayName,email
            Existing One,existing@example.com
            Alex Morgan,alex@example.com
            Alex Duplicate,alex@example.com
            """));

        result.TotalRows.Should().Be(3);
        result.ValidRows.Should().Be(1);
        result.DuplicateRows.Should().Be(2);
        result.InvalidRows.Should().Be(0);
        result.Rows.Count(row => row.IsDuplicate).Should().Be(2);
    }

    [Fact]
    public async Task ShouldCommitImportAndAssignSelectedTags()
    {
        var fixture = new ContactImportServiceFixture();
        var tagService = new TagService(fixture.TagRepository, fixture.UnitOfWork);
        var tag = await tagService.CreateAsync(new CreateTagRequest("Lead", "Audience"));

        var result = await fixture.Service.CommitAsync(new ContactImportCommitRequest(
            "contacts.csv",
            """
            displayName,email,source
            Alex Morgan,alex@example.com,CSV
            Sam Taylor,sam@example.com,CSV
            Sam Duplicate,sam@example.com,CSV
            """,
            [tag.Id]));

        result.TotalRows.Should().Be(3);
        result.CreatedCount.Should().Be(2);
        result.DuplicateCount.Should().Be(1);
        result.InvalidCount.Should().Be(0);

        fixture.ContactRepository.Contacts.Should().HaveCount(2);
        fixture.ContactRepository.Contacts.Should().OnlyContain(contact =>
            contact.Tags.Any(contactTag => contactTag.TagId == tag.Id));
        fixture.ImportJobRepository.ImportJobs.Should().ContainSingle(job =>
            job.CreatedCount == 2 &&
            job.DuplicateRows == 1);
        fixture.ContactActivityRepository.Activities.Should().HaveCount(2);
        fixture.ContactActivityRepository.Activities.Should().OnlyContain(activity =>
            activity.Type == ContactActivityType.ContactCreated &&
            activity.Subject == "Contact imported");
    }

    [Fact]
    public async Task ShouldRejectPreviewWhenRequiredHeaderIsMissing()
    {
        var fixture = new ContactImportServiceFixture();

        var act = () => fixture.Service.PreviewAsync(new ContactImportPreviewRequest(
            "contacts.csv",
            """
            name,email
            Alex Morgan,alex@example.com
            """));

        await act.Should().ThrowAsync<ApplicationValidationException>()
            .WithMessage("CSV is missing required header 'displayname'.");
    }

    private sealed class ContactImportServiceFixture
    {
        public ContactImportServiceFixture()
        {
            UnitOfWork = new InMemoryUnitOfWork();
            ContactRepository = new InMemoryContactRepository();
            TagRepository = new InMemoryTagRepository();
            ContactActivityRepository = new InMemoryContactActivityRepository();
            ImportJobRepository = new InMemoryImportJobRepository();
            CsvParser = new SimpleCsvParser();
            ContactActivityService = new ContactActivityService(ContactRepository, ContactActivityRepository);

            Service = new ContactImportService(
                CsvParser,
                ContactRepository,
                TagRepository,
                ImportJobRepository,
                ContactActivityService,
                UnitOfWork);
        }

        public InMemoryUnitOfWork UnitOfWork { get; }

        public InMemoryContactRepository ContactRepository { get; }

        public InMemoryTagRepository TagRepository { get; }

        public InMemoryContactActivityRepository ContactActivityRepository { get; }

        public InMemoryImportJobRepository ImportJobRepository { get; }

        public IContactImportCsvParser CsvParser { get; }

        public ContactActivityService ContactActivityService { get; }

        public ContactImportService Service { get; }
    }

    private sealed class SimpleCsvParser : IContactImportCsvParser
    {
        public ContactImportCsvParseResult Parse(string csvContent)
        {
            var lines = csvContent
                .Split(
                    ["\r\n", "\n", "\r"],
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (lines.Length == 0)
            {
                throw new InvalidOperationException("CSV content is required.");
            }

            var headers = lines[0]
                .Split(',', StringSplitOptions.TrimEntries)
                .Select(header => header.ToLowerInvariant())
                .ToArray();

            var rows = new List<ContactImportCsvRow>();

            for (var index = 1; index < lines.Length; index++)
            {
                var cells = lines[index].Split(',', StringSplitOptions.None);
                var values = new Dictionary<string, string?>(StringComparer.Ordinal);

                for (var columnIndex = 0; columnIndex < headers.Length; columnIndex++)
                {
                    var value = columnIndex < cells.Length ? cells[columnIndex] : null;
                    values[headers[columnIndex]] = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
                }

                rows.Add(new ContactImportCsvRow(index + 1, values));
            }

            return new ContactImportCsvParseResult(headers, rows);
        }
    }
}
