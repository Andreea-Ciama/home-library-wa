using HomeLibrary.Data;
using HomeLibrary.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddData(builder.Configuration);

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();