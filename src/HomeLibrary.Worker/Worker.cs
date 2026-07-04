using System.Text;
using System.Text.Json;
using HomeLibrary.Contracts.Messages;
using HomeLibrary.Contracts.Models;
using HomeLibrary.Data;
using HomeLibrary.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqOptions _options;

    public Worker(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqOptions> options)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password
        };

        IConnection? connection = null;

        while (connection is null && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                connection = factory.CreateConnection();
                Console.WriteLine("Worker connected to RabbitMQ.");
            }
            catch
            {
                Console.WriteLine("RabbitMQ not ready. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }

        if (connection is null)
            return;

        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += async (sender, args) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(args.Body.ToArray());

                var message = JsonSerializer.Deserialize<BookImportMessage>(json);

                if (message is null)
                    return;

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                db.Books.Add(new Book
                {
                    Id = Guid.NewGuid(),
                    Name = message.Name,
                    Author = message.Author,
                    Genre = message.Genre,
                    ImportDate = DateTime.UtcNow
                });

                await db.SaveChangesAsync(stoppingToken);

                Console.WriteLine($"Book imported: {message.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Worker error: {ex.Message}");
            }
        };

        channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: true,
            consumer: consumer
        );

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}