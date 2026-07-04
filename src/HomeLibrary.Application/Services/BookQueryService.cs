using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Dtos;

namespace HomeLibrary.Application.Services;

public class BookQueryService : IBookQueryService
{
    private readonly IBookRepository _repository;

    public BookQueryService(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookDto>> GetBooks()
    {
        var books = await _repository.GetAll();

        return books
            .Select(book => new BookDto(
                book.Id,
                book.Name,
                book.Author,
                book.Genre,
                book.ImportDate
            ))
            .ToList();
    }
}