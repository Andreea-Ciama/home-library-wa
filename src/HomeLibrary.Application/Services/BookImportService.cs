using System.Security.Cryptography;
using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Messages;
using HomeLibrary.Contracts.Models;
using HomeLibrary.Contracts.Responses;

namespace HomeLibrary.Application.Services;

public class BookImportService : IBookImportService
{
    private readonly IMessagePublisher _publisher;
    private readonly IImportHistoryRepository _importHistoryRepository;

    public BookImportService(
        IMessagePublisher publisher,
        IImportHistoryRepository importHistoryRepository)
    {
        _publisher = publisher;
        _importHistoryRepository = importHistoryRepository;
    }

    public async Task<ApiResponse<ImportResult>> Import(string filePath, string fileName)
    {
        var lines = await File.ReadAllLinesAsync(filePath);

        if (lines.Length == 0)
        {
            return ApiResponse<ImportResult>.Fail("The CSV file is empty. Please choose another file.");
        }

        var header = lines[0].Trim().ToLowerInvariant();

        if (header != "name,author,genre")
        {
            return ApiResponse<ImportResult>.Fail(
                "The CSV file has an invalid format. The first row must be: name,author,genre. Please choose another file."
            );
        }

        var dataLines = lines.Skip(1).ToArray();

        if (dataLines.Length == 0 || dataLines.All(string.IsNullOrWhiteSpace))
        {
            return ApiResponse<ImportResult>.Fail(
                "The CSV file does not contain any books. Please choose another file."
            );
        }

        var validationErrors = ValidateCsv(dataLines);

        if (validationErrors.Any())
        {
            return ApiResponse<ImportResult>.Fail(validationErrors.ToArray());
        }

        var fileHash = await ComputeFileHash(filePath);

        var alreadyImported = await _importHistoryRepository.ExistsByHash(fileHash);

        if (alreadyImported)
        {
            return ApiResponse<ImportResult>.Fail(
                "This CSV file was already uploaded. Please choose a different file."
            );
        }

        var queued = 0;

        foreach (var line in dataLines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            var name = parts[0].Trim();
            var author = parts[1].Trim();
            var genre = parts[2].Trim();

            var message = new BookImportMessage(name, author, genre);

            await _publisher.Publish(message);

            queued++;
        }

        await _importHistoryRepository.Add(new ImportHistory
        {
            Id = Guid.NewGuid(),
            FileName = fileName,
            FileHash = fileHash,
            ImportDate = DateTime.UtcNow
        });

        var result = new ImportResult(
            queued,
            $"Upload succeeded. {queued} book(s) were imported."
        );

        return ApiResponse<ImportResult>.Ok(result);
    }

    private static List<string> ValidateCsv(string[] lines)
    {
        var errors = new List<string>();
        var validRows = 0;

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');

            if (parts.Length != 3)
            {
                errors.Add(
                    $"Row {index + 2} is invalid. Each row must have exactly 3 columns: Name, Author, Genre."
                );
                continue;
            }

            var name = parts[0].Trim();
            var author = parts[1].Trim();
            var genre = parts[2].Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(genre))
            {
                errors.Add(
                    $"Row {index + 2} is invalid. Name, Author and Genre are required."
                );
                continue;
            }

            validRows++;
        }

        if (validRows == 0)
        {
            errors.Add("No valid books were found in the CSV file. Please choose another file.");
        }

        return errors;
    }

    private static async Task<string> ComputeFileHash(string filePath)
    {
        await using var stream = File.OpenRead(filePath);

        var hashBytes = await SHA256.HashDataAsync(stream);

        return Convert.ToHexString(hashBytes);
    }
}