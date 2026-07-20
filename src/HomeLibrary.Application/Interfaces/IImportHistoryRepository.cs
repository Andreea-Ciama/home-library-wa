using HomeLibrary.Contracts.Models;

namespace HomeLibrary.Application.Interfaces;

public interface IImportHistoryRepository
{
    Task<bool> ExistsByHash(
        string fileHash,
        CancellationToken cancellationToken = default);

    Task Add(
        ImportHistory importHistory,
        CancellationToken cancellationToken = default);
}