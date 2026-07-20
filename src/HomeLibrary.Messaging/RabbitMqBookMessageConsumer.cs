using System.Text;
using System.Text.Json;
using HomeLibrary.Contracts.Interfaces;
using HomeLibrary.Contracts.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HomeLibrary.Messaging;

public sealed class RabbitMqBookMessageConsumer(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqBookMessageConsumer> logger)
    : IBookMessageConsumer
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task Consume(
        Func<BookImportMessage, CancellationToken, Task<bool>> handler,
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            DispatchConsumersAsync = true
        };

        IConnection? connection = null;

        while (connection is null &&
               !cancellationToken.IsCancellationRequested)
        {
            try
            {
                connection = factory.CreateConnection();

                logger.LogInformation(
                    "Worker connected to RabbitMQ.");
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "RabbitMQ is unavailable. Retrying in 5 seconds.");

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);
            }
        }

        if (connection is null)
        {
            return;
        }

        using (connection)
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (_, args) =>
            {
                var succeeded = false;

                try
                {
                    var json = Encoding.UTF8.GetString(
                        args.Body.ToArray());

                    var message =
                        JsonSerializer.Deserialize<BookImportMessage>(
                            json);

                    if (message is null)
                    {
                        logger.LogWarning(
                            "A RabbitMQ message could not be deserialized.");
                    }
                    else
                    {
                        succeeded = await handler(
                            message,
                            cancellationToken);
                    }
                }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception,
                        "An error occurred while consuming a book message.");
                }

                if (succeeded)
                {
                    channel.BasicAck(
                        deliveryTag: args.DeliveryTag,
                        multiple: false);
                }
                else
                {
                    channel.BasicNack(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: false);
                }
            };

            channel.BasicConsume(
                queue: _options.QueueName,
                autoAck: false,
                consumer: consumer);

            try
            {
                await Task.Delay(
                    Timeout.Infinite,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation(
                    "RabbitMQ consumer is stopping.");
            }
        }
    }
}