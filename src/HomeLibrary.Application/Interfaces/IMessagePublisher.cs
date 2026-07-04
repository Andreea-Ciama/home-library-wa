using HomeLibrary.Contracts.Messages;

namespace HomeLibrary.Application.Interfaces;

public interface IMessagePublisher
{
    Task Publish(BookImportMessage message);
}