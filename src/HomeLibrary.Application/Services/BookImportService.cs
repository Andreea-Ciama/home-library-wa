using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Messages;

namespace HomeLibrary.Application.Services;

public class BookImportService : IBookImportService
{
    private readonly IMessagePublisher _publisher;

    public BookImportService(IMessagePublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task<int> Import(string filePath)
    {
        var lines = await File.ReadAllLinesAsync(filePath);

        var queued = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            if (parts.Length < 3)
                continue;

            var message = new BookImportMessage(
                parts[0].Trim(),
                parts[1].Trim(),
                parts[2].Trim()
            );

            await _publisher.Publish(message);
            queued++;
        }

        return queued;
    }
}