using RabbitMQ.Client;
using HomeLibrary.Contracts.Messages;
using System.Text;
using System.Text.Json;

namespace HomeLibrary.Api.Services;

public class RabbitMqPublisher
{
    private readonly ConnectionFactory _factory;

    public RabbitMqPublisher()
    {
        _factory = new ConnectionFactory()
        {
            HostName = "rabbitmq",
            UserName = "guest",
            Password = "guest"
        };
    }


   public async Task Publish(BookImportMessage message)
{
    await using var connection = await _factory.CreateConnectionAsync();
    await using var channel = await connection.CreateChannelAsync();

    const string queueName = "book-imports";

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
}
}