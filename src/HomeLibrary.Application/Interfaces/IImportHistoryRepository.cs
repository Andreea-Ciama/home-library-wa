using HomeLibrary.Contracts.Models;

namespace HomeLibrary.Application.Interfaces;

public interface IImportHistoryRepository
{
    Task<bool> ExistsByHash(string fileHash);

    Task Add(ImportHistory importHistory);
}