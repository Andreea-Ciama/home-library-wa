namespace HomeLibrary.Application.Validations;

public sealed class CsvContentValidator : ICsvContentValidator
{
    private static readonly string[] ExpectedHeader =
    {
        "name",
        "author",
        "genre"
    };

    public async Task<CsvValidationResult> ValidateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var result = new CsvValidationResult();

        var lines = await File.ReadAllLinesAsync(
            filePath,
            cancellationToken);

        if (lines.Length == 0)
        {
            result.Errors.Add(
                "The CSV file is empty. Please choose another file.");

            return result;
        }

        ValidateHeader(lines[0], result);

        if (!result.IsValid)
        {
            return result;
        }

        if (lines.Length == 1)
        {
            result.Errors.Add(
                "The CSV file does not contain any books.");

            return result;
        }

        for (var index = 1; index < lines.Length; index++)
        {
            var line = lines[index];

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            ValidateDataLine(
                line,
                index + 1,
                result);
        }

        if (result.DataLines.Count == 0 &&
            result.Errors.Count == 0)
        {
            result.Errors.Add(
                "The CSV file does not contain any valid books.");
        }

        return result;
    }

    private static void ValidateHeader(
        string headerLine,
        CsvValidationResult result)
    {
        var actualHeader = headerLine
            .Split(',')
            .Select(value => value.Trim().ToLowerInvariant())
            .ToArray();

        if (!actualHeader.SequenceEqual(ExpectedHeader))
        {
            result.Errors.Add(
                "The CSV header is invalid. The first row must be: name,author,genre.");
        }
    }

    private static void ValidateDataLine(
        string line,
        int rowNumber,
        CsvValidationResult result)
    {
        var columns = line.Split(',');

        if (columns.Length != 3)
        {
            result.Errors.Add(
                $"Row {rowNumber} is invalid. Each row must contain exactly three columns: Name, Author and Genre.");

            return;
        }

        var name = columns[0].Trim();
        var author = columns[1].Trim();
        var genre = columns[2].Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            result.Errors.Add(
                $"Row {rowNumber}: Name is required.");
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            result.Errors.Add(
                $"Row {rowNumber}: Author is required.");
        }

        if (string.IsNullOrWhiteSpace(genre))
        {
            result.Errors.Add(
                $"Row {rowNumber}: Genre is required.");
        }

        if (!string.IsNullOrWhiteSpace(name) &&
            !string.IsNullOrWhiteSpace(author) &&
            !string.IsNullOrWhiteSpace(genre))
        {
            result.DataLines.Add(line);
        }
    }
}