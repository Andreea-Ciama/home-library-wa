using System.Text;
using System.Text.Json;
using HomeLibrary.Worker.Data;
using HomeLibrary.Worker.Messages;
using HomeLibrary.Contracts.Models;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "rabbitmq",
            UserName = "guest",
            Password = "guest"
        };

        IConnection? connection = null;

        while (connection == null && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Connecting to RabbitMQ...");

                connection = factory.CreateConnection();

                Console.WriteLine("Connected to RabbitMQ.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RabbitMQ not ready: {ex.Message}");
                await Task.Delay(5000, stoppingToken);
            }
        }

        if (connection == null)
            return;

        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "book-imports",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Received: {json}");

                var message = JsonSerializer.Deserialize<BookImportMessage>(json);

                if (message == null)
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

                db.SaveChanges();

                Console.WriteLine($"Saved: {message.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        };

        channel.BasicConsume(
            queue: "book-imports",
            autoAck: true,
            consumer: consumer
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}