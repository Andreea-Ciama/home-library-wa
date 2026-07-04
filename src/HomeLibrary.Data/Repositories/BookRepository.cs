using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Data.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;

    public BookRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public Task<List<Book>> GetAll()
    {
        return _db.Books
            .OrderByDescending(book => book.ImportDate)
            .ToListAsync();
    }
}