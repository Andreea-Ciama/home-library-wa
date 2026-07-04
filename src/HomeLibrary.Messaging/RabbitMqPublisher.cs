using System.Text;
using System.Text.Json;
using HomeLibrary.Application.Interfaces;
using HomeLibrary.Contracts.Messages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace HomeLibrary.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly RabbitMqOptions _options;

    public RabbitMqPublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public Task Publish(BookImportMessage message)
    {
        Console.WriteLine($"RabbitMQ HostName: '{_options.HostName}'");
        Console.WriteLine($"RabbitMQ UserName: '{_options.UserName}'");
        Console.WriteLine($"RabbitMQ QueueName: '{_options.QueueName}'");

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
            arguments: null
        );

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: _options.QueueName,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }
}