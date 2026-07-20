using HomeLibrary.Api.Requests;

namespace HomeLibrary.Api.Validations;

public sealed class ImportBooksRequestValidator
    : RequestValidator<ImportBooksRequest>
{
    private const long MaximumFileSize = 10 * 1024 * 1024;

    public override Task<IReadOnlyCollection<string>> ValidateAsync(
        ImportBooksRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (request.File is null)
        {
            errors.Add("Please select a CSV file.");

            return Task.FromResult<IReadOnlyCollection<string>>(errors);
        }

        if (request.File.Length == 0)
        {
            errors.Add("The selected file is empty.");
        }

        if (request.File.Length > MaximumFileSize)
        {
            errors.Add("The selected file is too large. Maximum size is 10 MB.");
        }

        var extension = Path.GetExtension(request.File.FileName);

        if (!string.Equals(
                extension,
                ".csv",
                StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Only CSV files are allowed. Please choose another file.");
        }

        return Task.FromResult<IReadOnlyCollection<string>>(errors);
    }
}