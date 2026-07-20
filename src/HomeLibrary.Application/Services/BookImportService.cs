using System.Security.Cryptography;
using HomeLibrary.Application.Exceptions;
using HomeLibrary.Application.Interfaces;
using HomeLibrary.Application.Validations;
using HomeLibrary.Contracts.Interfaces;
using HomeLibrary.Contracts.Messages;
using HomeLibrary.Contracts.Models;
using HomeLibrary.Contracts.Responses;

namespace HomeLibrary.Application.Services;

public sealed class BookImportService(
    IMessagePublisher publisher,
    IImportHistoryRepository importHistoryRepository,
    ICsvContentValidator csvValidator)
    : IBookImportService
{
    public async Task<ImportResult> Import(
        string filePath,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var validationResult =
            await csvValidator.ValidateAsync(
                filePath,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new CsvValidationException(
                validationResult.Errors);
        }

        var fileHash = await ComputeFileHash(
            filePath,
            cancellationToken);

        var alreadyImported =
            await importHistoryRepository.ExistsByHash(
                fileHash,
                cancellationToken);

        if (alreadyImported)
        {
            throw new DuplicateImportException(
                "This CSV file was already uploaded. Please choose a different file.");
        }

        var importId = Guid.NewGuid();

        var import = new ImportHistory
        {
            Id = importId,
            FileName = fileName,
            FileHash = fileHash,
            ImportDate = DateTime.UtcNow,
            Status = ImportStatus.Pending,
            TotalBooks = validationResult.DataLines.Count,
            ProcessedBooks = 0,
            ImportedBooks = 0,
            FailedBooks = 0
        };

        await importHistoryRepository.Add(
            import,
            cancellationToken);

        foreach (var line in validationResult.DataLines)
        {
            var columns = line.Split(',');

            var message = new BookImportMessage(
                ImportId: importId,
                Name: columns[0].Trim(),
                Author: columns[1].Trim(),
                Genre: columns[2].Trim());

            await publisher.Publish(
                message,
                cancellationToken);
        }

        return new ImportResult(
            validationResult.DataLines.Count,
            $"Upload succeeded. {validationResult.DataLines.Count} book(s) were queued for import.");
    }

    private static async Task<string> ComputeFileHash(
        string filePath,
        CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(filePath);

        var hashBytes = await SHA256.HashDataAsync(
            stream,
            cancellationToken);

        return Convert.ToHexString(hashBytes);
    }
}