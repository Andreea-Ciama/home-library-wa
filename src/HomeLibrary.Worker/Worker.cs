using System.Text;
using System.Text.Json;
using HomeLibrary.Contracts.Messages;
using HomeLibrary.Contracts.Models;
using HomeLibrary.Worker.Data;
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
        var factory = new ConnectionFactory
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
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
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

                var message = JsonSerializer.Deserialize<BookImportMessage>(json);

                if (message == null)
                    return;

                SaveBookWithRetry(message);
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

    private void SaveBookWithRetry(BookImportMessage message)
    {
        const int maxRetries = 5;

        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
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

                Console.WriteLine($"Saved book: {message.Name}");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                Console.WriteLine($"Postgres not ready or save failed: {ex.Message}");
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
        }

        throw new Exception($"Could not save book after retries: {message.Name}");
    }
}