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

        var isCsv =
            file.ContentType == "text/csv" ||
            file.ContentType == "application/vnd.ms-excel" ||
            file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);

        if (!isCsv)
            return BadRequest("Invalid file type. Please upload a CSV file.");

        using var reader = new StreamReader(file.OpenReadStream());

        var header = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(header))
            return BadRequest("CSV is empty.");

        var headerParts = header.Split(',').Select(x => x.Trim().ToLower()).ToArray();

        if (headerParts.Length < 3 ||
            headerParts[0] != "name" ||
            headerParts[1] != "author" ||
            headerParts[2] != "genre")
        {
            return BadRequest("Invalid CSV header. Expected: name,author,genre");
        }

        var queued = 0;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            if (parts.Length < 3)
                continue;

            var name = parts[0].Trim();
            var author = parts[1].Trim();
            var genre = parts[2].Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(genre))
            {
                continue;
            }

            var message = new BookImportMessage(name, author, genre);

            await _publisher.Publish(message);
            queued++;
        }

        if (queued == 0)
            return BadRequest("CSV contains no valid rows.");

        return Accepted(new { queued });
    }
}