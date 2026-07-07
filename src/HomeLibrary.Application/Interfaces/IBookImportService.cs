using HomeLibrary.Contracts.Responses;

namespace HomeLibrary.Application.Interfaces;

public interface IBookImportService
{
    Task<ApiResponse<ImportResult>> Import(string filePath, string fileName);
}