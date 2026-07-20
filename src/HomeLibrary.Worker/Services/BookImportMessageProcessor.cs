using HomeLibrary.Contracts.Messages;
using HomeLibrary.Contracts.Models;
using HomeLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Worker.Services;

public sealed class BookImportMessageProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<BookImportMessageProcessor> logger)
{
    public async Task<bool> Process(
        BookImportMessage message,
        CancellationToken cancellationToken)
    {
        await using var scope =
            scopeFactory.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<LibraryDbContext>();

        var import = await db.ImportHistories
            .SingleOrDefaultAsync(
                currentImport =>
                    currentImport.Id == message.ImportId,
                cancellationToken);

        if (import is null)
        {
            logger.LogError(
                "Import {ImportId} was not found.",
                message.ImportId);

            return false;
        }

        if (import.Status == ImportStatus.Pending)
        {
            import.Status = ImportStatus.Processing;
            import.StartedAt = DateTime.UtcNow;
        }

        try
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Name = message.Name,
                Author = message.Author,
                Genre = message.Genre,
                ImportDate = DateTime.UtcNow,
                ImportId = message.ImportId
            };

            await db.Books.AddAsync(
                book,
                cancellationToken);

            import.ImportedBooks++;
            import.ProcessedBooks++;

            UpdateFinalStatus(import);

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Book imported: {BookName}, ImportId: {ImportId}",
                message.Name,
                message.ImportId);

            return true;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Book {BookName} could not be imported.",
                message.Name);

            import.FailedBooks++;
            import.ProcessedBooks++;

            UpdateFinalStatus(import);

            await db.SaveChangesAsync(cancellationToken);

            /*
             * Mesajul a fost gestionat, chiar dacă respectiva carte
             * nu a putut fi salvată. Întoarcem true pentru ACK,
             * altfel mesajul ar putea fi procesat repetat.
             */
            return true;
        }
    }

    private static void UpdateFinalStatus(
        ImportHistory import)
    {
        if (import.ProcessedBooks < import.TotalBooks)
        {
            import.Status = ImportStatus.Processing;
            return;
        }

        import.CompletedAt = DateTime.UtcNow;

        if (import.ImportedBooks == 0)
        {
            import.Status = ImportStatus.Failed;
        }
        else if (import.FailedBooks > 0)
        {
            import.Status =
                ImportStatus.ImportedWithFailures;
        }
        else
        {
            import.Status = ImportStatus.Imported;
        }
    }
}