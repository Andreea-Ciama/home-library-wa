using Microsoft.AspNetCore.Mvc;
using HomeLibrary.Api.Services;
using HomeLibrary.Contracts.Messages;

namespace HomeLibrary.Api.Controllers;

[ApiController]
[Route("api/imports")]
public class ImportsController : ControllerBase
{
    private readonly RabbitMqPublisher _publisher;

    public ImportsController(RabbitMqPublisher publisher)
    {
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        using var reader = new StreamReader(file.OpenReadStream());

        var lineNumber = 0;
        var queued = 0;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            // skip header
            if (lineNumber == 0)
            {
                lineNumber++;
                continue;
            }

            var parts = line.Split(',');

            if (parts.Length < 3)
            {
                lineNumber++;
                continue;
            }

            var message = new BookImportMessage(
                parts[0].Trim(),
                parts[1].Trim(),
                parts[2].Trim()
            );

            await _publisher.Publish(message);
            queued++;

            lineNumber++;
        }

        return Accepted(new { queued });
    }
}