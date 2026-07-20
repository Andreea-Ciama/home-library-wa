namespace HomeLibrary.Application.Exceptions;

public sealed class CsvValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public CsvValidationException(
        IReadOnlyCollection<string> errors)
        : base("The CSV file is invalid.")
    {
        Errors = errors;
    }
}