using HomeLibrary.Contracts.Interfaces;
using HomeLibrary.Worker.Services;

public sealed class Worker(
        IBookMessageConsumer consumer,
        BookImportMessageProcessor processor,
        ILogger<Worker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Book import worker started.");

        await consumer.Consume(
            processor.Process,
            stoppingToken);
    }
}