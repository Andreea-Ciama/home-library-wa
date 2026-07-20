using HomeLibrary.Api.Requests;
using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HomeLibrary.Api.Controllers;

[ApiController]
[Route("api/imports")]
public sealed class ImportsController(
        IBookImportService bookImportService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Import(
        [FromForm] ImportBooksRequest request,
        CancellationToken cancellationToken)
    {
        var tempFilePath = Path.GetTempFileName();

        try
        {
            await using (var stream = new FileStream(
                             tempFilePath,
                             FileMode.Create,
                             FileAccess.Write,
                             FileShare.None,
                             bufferSize: 81920,
                             useAsync: true))
            {
                await request.File!.CopyToAsync(
                    stream,
                    cancellationToken);
            }

            var result = await bookImportService.Import(
                tempFilePath,
                request.File!.FileName,
                cancellationToken);

            return Ok(
                ApiResponse<ImportResult>.Ok(result));
        }
        finally
        {
            if (System.IO.File.Exists(tempFilePath))
            {
                System.IO.File.Delete(tempFilePath);
            }
        }
    }
}