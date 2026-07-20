using System.Text;
using System.Text.Json;
using HomeLibrary.Contracts.Interfaces;
using HomeLibrary.Contracts.Messages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HomeLibrary.Messaging;

public sealed class RabbitMqPublisher(
        IOptions<RabbitMqOptions> options)
    : IMessagePublisher
{
    private readonly RabbitMqOptions _options = options.Value;

    public Task Publish(
        BookImportMessage message,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: "",
            routingKey: _options.QueueName,
            basicProperties: properties,
            body: body);

        return Task.CompletedTask;
    }
}