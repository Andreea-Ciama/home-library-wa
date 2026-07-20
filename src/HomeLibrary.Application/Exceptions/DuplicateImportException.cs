namespace HomeLibrary.Application.Exceptions;

public sealed class DuplicateImportException : Exception
{
    public DuplicateImportException(string message)
        : base(message)
    {
    }
}