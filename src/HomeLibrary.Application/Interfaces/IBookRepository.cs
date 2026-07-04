using HomeLibrary.Contracts.Models;

namespace HomeLibrary.Application.Interfaces;

public interface IBookRepository
{
    Task<List<Book>> GetAll();
}