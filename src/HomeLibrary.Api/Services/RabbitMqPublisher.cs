using System.Text;
using System.Text.Json;
using HomeLibrary.Contracts.Messages;
using RabbitMQ.Client;

namespace HomeLibrary.Api.Services;

public class RabbitMqPublisher
{
    private readonly ConnectionFactory _factory;

    public RabbitMqPublisher()
    {
        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            UserName = "guest",
            Password = "guest"
        };
    }

    public async Task Publish(BookImportMessage message)
    {
        const int maxRetries = 5;
        const string queueName = "book-imports";

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await using var connection = await _factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    body: body
                );

                return;
            }
            catch when (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }

        throw new Exception("RabbitMQ is not reachable after multiple retries.");
    }
}