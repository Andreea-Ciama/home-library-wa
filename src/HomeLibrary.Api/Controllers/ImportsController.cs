using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Responses;
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
        {
            return BadRequest(ApiResponse<ImportResult>.Fail("File is required."));
        }

        var extension = Path.GetExtension(file.FileName);

        if (!string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ApiResponse<ImportResult>.Fail("Only CSV files are allowed."));
        }

        var tempFile = Path.GetTempFileName();

        try
        {
            await using (var stream = System.IO.File.Create(tempFile))
            {
                await file.CopyToAsync(stream);
            }

            var response = await _bookImportService.Import(tempFile, file.FileName);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        finally
        {
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }
        }
    }
}