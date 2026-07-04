using HomeLibrary.Contracts.Dtos;

namespace HomeLibrary.Application.Interfaces;

public interface IBookQueryService
{
    Task<List<BookDto>> GetBooks();
}