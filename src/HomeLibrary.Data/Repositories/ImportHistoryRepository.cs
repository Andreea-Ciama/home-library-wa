using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Data.Repositories;

public class ImportHistoryRepository : IImportHistoryRepository
{
    private readonly LibraryDbContext _db;

    public ImportHistoryRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsByHash(string fileHash)
    {
        return _db.ImportHistories.AnyAsync(x => x.FileHash == fileHash);
    }

    public async Task Add(ImportHistory importHistory)
    {
        _db.ImportHistories.Add(importHistory);
        await _db.SaveChangesAsync();
    }
}