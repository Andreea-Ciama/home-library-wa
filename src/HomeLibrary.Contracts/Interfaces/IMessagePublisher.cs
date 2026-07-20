using HomeLibrary.Contracts.Messages;

namespace HomeLibrary.Contracts.Interfaces;

public interface IMessagePublisher
{
    Task Publish(
        BookImportMessage message,
        CancellationToken cancellationToken = default);
}