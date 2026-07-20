using HomeLibrary.Contracts.Responses;

namespace HomeLibrary.Application.Interfaces;

public interface IBookImportService
{
    Task<ImportResult> Import(
        string filePath,
        string fileName,
        CancellationToken cancellationToken = default);
}