using System.Text;
using System.Text.Json;
using HomeLibrary.Worker.Data;
using HomeLibrary.Worker.Messages;
using HomeLibrary.Worker.Models;
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

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "rabbitmq", 
            UserName = "guest",
            Password = "guest"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

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

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                db.Books.Add(new Book
                {
                    Name = message.Name,
                    Author = message.Author,
                    Genre = message.Genre,
                    ImportDate = DateTime.UtcNow
                });

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Worker error: {ex.Message}");
            }
        };

        channel.BasicConsume(
            queue: "book-imports",
            autoAck: true,
            consumer: consumer
        );

        return Task.CompletedTask;
    }
}