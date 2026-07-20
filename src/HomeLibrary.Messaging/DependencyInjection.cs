using HomeLibrary.Contracts.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLibrary.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RabbitMqOptions>()
            .Bind(
                configuration.GetSection(
                    RabbitMqOptions.SectionName));
            

        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

        services.AddSingleton<
            IBookMessageConsumer,
            RabbitMqBookMessageConsumer>();

        return services;
    }
}