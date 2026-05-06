namespace OutreachFlow.Application.ContactImports;

public interface IContactImportCsvParser
{
    ContactImportCsvParseResult Parse(string csvContent);
}

public sealed record ContactImportCsvParseResult(
    IReadOnlyList<string> Headers,
    IReadOnlyList<ContactImportCsvRow> Rows);

public sealed record ContactImportCsvRow(
    int RowNumber,
    IReadOnlyDictionary<string, string?> Values);
