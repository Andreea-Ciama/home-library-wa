using System.ComponentModel.DataAnnotations;

namespace HomeLibrary.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    [Required]
    public string HostName { get; init; } = "";

    [Required]
    public string UserName { get; init; } = "";

    [Required]
    public string Password { get; init; } = "";

    [Required]
    public string QueueName { get; init; } = "";
}