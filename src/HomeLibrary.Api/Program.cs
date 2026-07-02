using HomeLibrary.Api.Data;
using Microsoft.EntityFrameworkCore;
using HomeLibrary.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

builder.Services.AddSingleton<RabbitMqPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Angular");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();