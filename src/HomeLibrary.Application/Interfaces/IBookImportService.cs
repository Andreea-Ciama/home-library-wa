namespace HomeLibrary.Application.Interfaces;

public interface IBookImportService
{
    Task<int> Import(string filePath);
}