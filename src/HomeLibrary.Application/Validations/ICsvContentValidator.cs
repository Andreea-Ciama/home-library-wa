namespace HomeLibrary.Application.Validations;

public interface ICsvContentValidator
{
    Task<CsvValidationResult> ValidateAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}