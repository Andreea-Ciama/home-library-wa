using HomeLibrary.Api.Filters;
using HomeLibrary.Api.Middlewares;
using HomeLibrary.Api.Validations;
using HomeLibrary.Application;
using HomeLibrary.Data;
using HomeLibrary.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddScoped<IRequestValidator, ImportBooksRequestValidator>();
builder.Services.AddScoped<IRequestValidatorFactory, RequestValidatorFactory>();
builder.Services.AddScoped<RequestValidationFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<RequestValidationFilter>();
});

builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddData(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext =
        scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Angular");

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();