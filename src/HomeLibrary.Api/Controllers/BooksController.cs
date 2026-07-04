using HomeLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookQueryService _bookQueryService;

    public BooksController(IBookQueryService bookQueryService)
    {
        _bookQueryService = bookQueryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await _bookQueryService.GetBooks();

        return Ok(books);
    }
}