using HomeLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController(
        IBookQueryService bookQueryService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var books = await bookQueryService.GetBooks();

        return Ok(books);
    }
}