using HomeLibrary.Contracts.Messages;

namespace HomeLibrary.Contracts.Interfaces;

public interface IBookMessageConsumer
{
    Task Consume(
        Func<BookImportMessage, CancellationToken, Task<bool>> handler,
        CancellationToken cancellationToken);
}