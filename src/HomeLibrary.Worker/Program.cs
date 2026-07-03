using HomeLibrary.Worker;
using HomeLibrary.Worker.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();