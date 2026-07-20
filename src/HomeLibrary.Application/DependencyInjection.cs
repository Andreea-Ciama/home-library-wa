using HomeLibrary.Application.Interfaces;
using HomeLibrary.Application.Services;
using HomeLibrary.Application.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace HomeLibrary.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IBookImportService, BookImportService>();
        services.AddScoped<IBookQueryService, BookQueryService>();

        services.AddScoped<ICsvContentValidator, CsvContentValidator>();

        return services;
    }
}