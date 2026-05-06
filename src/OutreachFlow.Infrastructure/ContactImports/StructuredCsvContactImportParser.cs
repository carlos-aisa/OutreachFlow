using System.Text;
using OutreachFlow.Application.ContactImports;

namespace OutreachFlow.Infrastructure.ContactImports;

public sealed class StructuredCsvContactImportParser : IContactImportCsvParser
{
    public ContactImportCsvParseResult Parse(string csvContent)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
        {
            throw new InvalidOperationException("CSV content is required.");
        }

        var matrix = ParseMatrix(csvContent);

        if (matrix.Count == 0)
        {
            throw new InvalidOperationException("CSV content does not contain rows.");
        }

        var headers = matrix[0]
            .Select(NormalizeHeader)
            .ToArray();

        if (headers.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException("CSV header contains empty column names.");
        }

        var rows = new List<ContactImportCsvRow>();

        for (var rowIndex = 1; rowIndex < matrix.Count; rowIndex++)
        {
            var cells = matrix[rowIndex];

            if (cells.Count > headers.Length &&
                cells.Skip(headers.Length).Any(cell => !string.IsNullOrWhiteSpace(cell)))
            {
                throw new InvalidOperationException(
                    $"CSV row {rowIndex + 1} contains more columns than the header.");
            }

            var values = new Dictionary<string, string?>(StringComparer.Ordinal);

            for (var columnIndex = 0; columnIndex < headers.Length; columnIndex++)
            {
                var raw = columnIndex < cells.Count ? cells[columnIndex] : null;
                values[headers[columnIndex]] = string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
            }

            rows.Add(new ContactImportCsvRow(
                rowIndex + 1,
                values));
        }

        return new ContactImportCsvParseResult(headers, rows);
    }

    private static List<List<string>> ParseMatrix(string csvContent)
    {
        var rows = new List<List<string>>();
        var currentRow = new List<string>();
        var currentField = new StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < csvContent.Length; index++)
        {
            var current = csvContent[index];

            if (inQuotes)
            {
                if (current == '"')
                {
                    if (index + 1 < csvContent.Length && csvContent[index + 1] == '"')
                    {
                        currentField.Append('"');
                        index++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    currentField.Append(current);
                }

                continue;
            }

            switch (current)
            {
                case '"':
                    inQuotes = true;
                    break;
                case ',':
                    currentRow.Add(currentField.ToString());
                    currentField.Clear();
                    break;
                case '\r':
                    if (index + 1 < csvContent.Length && csvContent[index + 1] == '\n')
                    {
                        index++;
                    }

                    EndRow(rows, currentRow, currentField);
                    break;
                case '\n':
                    EndRow(rows, currentRow, currentField);
                    break;
                default:
                    currentField.Append(current);
                    break;
            }
        }

        if (inQuotes)
        {
            throw new InvalidOperationException("CSV content has an unterminated quoted value.");
        }

        EndRow(rows, currentRow, currentField);

        while (rows.Count > 0 && rows[^1].All(string.IsNullOrWhiteSpace))
        {
            rows.RemoveAt(rows.Count - 1);
        }

        return rows;
    }

    private static void EndRow(
        List<List<string>> rows,
        List<string> currentRow,
        StringBuilder currentField)
    {
        currentRow.Add(currentField.ToString());
        currentField.Clear();

        if (currentRow.Count == 1 && string.IsNullOrWhiteSpace(currentRow[0]))
        {
            currentRow.Clear();
            return;
        }

        rows.Add([.. currentRow]);
        currentRow.Clear();
    }

    private static string NormalizeHeader(string header)
    {
        return string.IsNullOrWhiteSpace(header)
            ? string.Empty
            : header.Trim().ToLowerInvariant();
    }
}
