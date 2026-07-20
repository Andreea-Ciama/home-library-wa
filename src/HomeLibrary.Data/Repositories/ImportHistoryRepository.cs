using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Data.Repositories;

public sealed class ImportHistoryRepository(
        LibraryDbContext db)
    : IImportHistoryRepository
{
    public Task<bool> ExistsByHash(
        string fileHash,
        CancellationToken cancellationToken = default)
    {
        return db.ImportHistories.AnyAsync(
            importHistory =>
                importHistory.FileHash == fileHash,
            cancellationToken);
    }

    public async Task Add(
        ImportHistory importHistory,
        CancellationToken cancellationToken = default)
    {
        await db.ImportHistories.AddAsync(
            importHistory,
            cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}