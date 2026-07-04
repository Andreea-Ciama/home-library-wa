using HomeLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Api.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly IBookImportService _bookImportService;

    public ImportsController(IBookImportService bookImportService)
    {
        _bookImportService = bookImportService;
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        var tempFile = Path.GetTempFileName();

        await using (var stream = System.IO.File.Create(tempFile))
        {
            await file.CopyToAsync(stream);
        }

        var queued = await _bookImportService.Import(tempFile);

        System.IO.File.Delete(tempFile);

        return Ok(new { queued });
    }
}