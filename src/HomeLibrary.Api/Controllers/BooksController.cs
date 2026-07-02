using HomeLibrary.Api.Data;
using HomeLibrary.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _db;

    public BooksController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IEnumerable<BookDto>> Get()
    {
        return await _db.Books
            .OrderByDescending(x => x.ImportDate)
            .Select(x => new BookDto(
                x.Name,
                x.Author,
                x.Genre,
                x.ImportDate))
            .ToListAsync();
    }
}