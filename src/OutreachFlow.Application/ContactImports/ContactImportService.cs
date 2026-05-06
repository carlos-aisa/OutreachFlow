using OutreachFlow.Application.Common;
using OutreachFlow.Application.Contacts;
using OutreachFlow.Application.ContactActivities;
using OutreachFlow.Application.Tags;
using OutreachFlow.Domain.Common;
using OutreachFlow.Domain.ContactActivities;
using OutreachFlow.Domain.ContactImports;
using OutreachFlow.Domain.Contacts;
using OutreachFlow.Domain.Tags;

namespace OutreachFlow.Application.ContactImports;

public sealed class ContactImportService(
    IContactImportCsvParser csvParser,
    IContactRepository contactRepository,
    ITagRepository tagRepository,
    IImportJobRepository importJobRepository,
    IContactActivityService contactActivityService,
    IUnitOfWork unitOfWork) : IContactImportService
{
    private static readonly string[] RequiredHeaders =
    [
        "displayname",
        "email"
    ];

    public async Task<ContactImportPreviewResult> PreviewAsync(
        ContactImportPreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var evaluation = await EvaluateRowsAsync(request.FileName, request.CsvContent, cancellationToken);

        return new ContactImportPreviewResult(
            evaluation.TotalRows,
            evaluation.ValidRows,
            evaluation.DuplicateRows,
            evaluation.InvalidRows,
            evaluation.Rows);
    }

    public async Task<ContactImportCommitResult> CommitAsync(
        ContactImportCommitRequest request,
        CancellationToken cancellationToken = default)
    {
        var evaluation = await EvaluateRowsAsync(request.FileName, request.CsvContent, cancellationToken);
        var tags = await LoadTagsAsync(request.TagIds ?? [], cancellationToken);

        var importJob = new ImportJob(
            request.FileName,
            evaluation.TotalRows,
            evaluation.ValidRows,
            evaluation.DuplicateRows,
            evaluation.InvalidRows);
        await importJobRepository.AddAsync(importJob, cancellationToken);

        var createdCount = 0;
        var duplicateCount = evaluation.DuplicateRows;

        foreach (var row in evaluation.CreatableRows)
        {
            var existingContact = await contactRepository.GetByNormalizedEmailAsync(
                row.NormalizedEmail,
                cancellationToken);

            if (existingContact is not null)
            {
                duplicateCount++;
                continue;
            }

            Contact contact;

            try
            {
                contact = new Contact(
                    row.DisplayName,
                    row.Email,
                    null,
                    row.Phone,
                    row.Role,
                    row.Source,
                    ContactStatus.New,
                    false);
            }
            catch (DomainException exception)
            {
                throw new ApplicationValidationException(exception.Message);
            }

            foreach (var tag in tags)
            {
                contact.AssignTag(tag.Id);
            }

            await contactRepository.AddAsync(contact, cancellationToken);
            await contactActivityService.RecordAsync(new CreateContactActivityRequest(
                contact.Id,
                contact.OrganizationId,
                ContactActivityType.ContactCreated,
                Subject: "Contact imported",
                BodyPreview: null,
                MetadataJson: null),
                cancellationToken);
            createdCount++;
        }

        importJob.MarkCompleted(
            createdCount,
            duplicateCount,
            evaluation.InvalidRows,
            DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ContactImportCommitResult(
            importJob.Id,
            evaluation.TotalRows,
            createdCount,
            duplicateCount,
            evaluation.InvalidRows);
    }

    public async Task<IReadOnlyList<ImportJobDto>> ListJobsAsync(
        int? limit,
        CancellationToken cancellationToken = default)
    {
        var jobs = await importJobRepository.ListAsync(limit, cancellationToken);

        return jobs.Select(Map).ToArray();
    }

    private async Task<RowEvaluation> EvaluateRowsAsync(
        string fileName,
        string csvContent,
        CancellationToken cancellationToken)
    {
        ValidateInput(fileName, csvContent);

        ContactImportCsvParseResult parsedRows;

        try
        {
            parsedRows = csvParser.Parse(csvContent);
        }
        catch (InvalidOperationException exception)
        {
            throw new ApplicationValidationException(exception.Message);
        }

        EnsureRequiredHeaders(parsedRows.Headers);

        var rows = new List<ContactImportPreviewRowDto>(parsedRows.Rows.Count);
        var creatableRows = new List<CreatableContactImportRow>();
        var seenEmails = new HashSet<string>(StringComparer.Ordinal);
        var duplicateRows = 0;
        var invalidRows = 0;

        foreach (var row in parsedRows.Rows)
        {
            var displayName = GetValue(row.Values, "displayname");
            var email = GetValue(row.Values, "email");
            var phone = GetValue(row.Values, "phone");
            var role = GetValue(row.Values, "role");
            var source = GetValue(row.Values, "source");

            var validationMessage = ValidateRequiredFields(displayName, email, out var normalizedEmail);

            if (validationMessage is not null)
            {
                rows.Add(new ContactImportPreviewRowDto(
                    row.RowNumber,
                    displayName,
                    email,
                    phone,
                    role,
                    source,
                    IsValid: false,
                    IsDuplicate: false,
                    Message: validationMessage));
                invalidRows++;
                continue;
            }

            if (!seenEmails.Add(normalizedEmail!))
            {
                rows.Add(new ContactImportPreviewRowDto(
                    row.RowNumber,
                    displayName,
                    email,
                    phone,
                    role,
                    source,
                    IsValid: false,
                    IsDuplicate: true,
                    Message: "Duplicate email found in the CSV file."));
                duplicateRows++;
                continue;
            }

            var existingContact = await contactRepository.GetByNormalizedEmailAsync(
                normalizedEmail!,
                cancellationToken);

            if (existingContact is not null)
            {
                rows.Add(new ContactImportPreviewRowDto(
                    row.RowNumber,
                    displayName,
                    email,
                    phone,
                    role,
                    source,
                    IsValid: false,
                    IsDuplicate: true,
                    Message: "A contact with this email already exists."));
                duplicateRows++;
                continue;
            }

            rows.Add(new ContactImportPreviewRowDto(
                row.RowNumber,
                displayName,
                email,
                phone,
                role,
                source,
                IsValid: true,
                IsDuplicate: false,
                Message: null));
            creatableRows.Add(new CreatableContactImportRow(
                displayName!,
                email!,
                normalizedEmail!,
                phone,
                role,
                source));
        }

        var validRows = rows.Count - duplicateRows - invalidRows;

        return new RowEvaluation(
            rows.Count,
            validRows,
            duplicateRows,
            invalidRows,
            rows,
            creatableRows);
    }

    private async Task<IReadOnlyList<Tag>> LoadTagsAsync(
        IReadOnlyList<Guid> tagIds,
        CancellationToken cancellationToken)
    {
        if (tagIds.Count == 0)
        {
            return [];
        }

        var uniqueTagIds = tagIds.Distinct().ToArray();
        var tags = new List<Tag>(uniqueTagIds.Length);

        foreach (var tagId in uniqueTagIds)
        {
            var tag = await tagRepository.GetByIdAsync(tagId, cancellationToken)
                ?? throw new ApplicationNotFoundException("Tag was not found.");
            tags.Add(tag);
        }

        return tags;
    }

    private static ImportJobDto Map(ImportJob importJob)
    {
        return new ImportJobDto(
            importJob.Id,
            importJob.FileName,
            importJob.Status,
            importJob.TotalRows,
            importJob.ValidRows,
            importJob.CreatedCount,
            importJob.DuplicateRows,
            importJob.InvalidRows,
            importJob.FailureReason,
            importJob.CreatedAt,
            importJob.CompletedAt);
    }

    private static void ValidateInput(string fileName, string csvContent)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ApplicationValidationException("Import file name is required.");
        }

        if (string.IsNullOrWhiteSpace(csvContent))
        {
            throw new ApplicationValidationException("CSV content is required.");
        }
    }

    private static void EnsureRequiredHeaders(IReadOnlyList<string> headers)
    {
        foreach (var requiredHeader in RequiredHeaders)
        {
            if (!headers.Contains(requiredHeader, StringComparer.Ordinal))
            {
                throw new ApplicationValidationException(
                    $"CSV is missing required header '{requiredHeader}'.");
            }
        }
    }

    private static string? ValidateRequiredFields(
        string? displayName,
        string? email,
        out string? normalizedEmail)
    {
        normalizedEmail = null;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return "Display name is required.";
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return "Email is required.";
        }

        try
        {
            normalizedEmail = EmailAddress.Normalize(email);
            _ = EmailAddress.RequireValid(email);
        }
        catch (DomainException exception)
        {
            return exception.Message;
        }

        return null;
    }

    private static string? GetValue(
        IReadOnlyDictionary<string, string?> values,
        string key)
    {
        if (!values.TryGetValue(key, out var value))
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private sealed record RowEvaluation(
        int TotalRows,
        int ValidRows,
        int DuplicateRows,
        int InvalidRows,
        IReadOnlyList<ContactImportPreviewRowDto> Rows,
        IReadOnlyList<CreatableContactImportRow> CreatableRows);

    private sealed record CreatableContactImportRow(
        string DisplayName,
        string Email,
        string NormalizedEmail,
        string? Phone,
        string? Role,
        string? Source);
}
