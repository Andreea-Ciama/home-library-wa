using HomeLibrary.Application.Interfaces;
using HomeLibrary.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLibrary.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBookQueryService, BookQueryService>();
        services.AddScoped<IBookImportService, BookImportService>();

        return services;
    }
}