using HomeLibrary.Data;
using HomeLibrary.Messaging;
using HomeLibrary.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddData(builder.Configuration);
builder.Services.AddMessaging(builder.Configuration);

builder.Services.AddSingleton<BookImportMessageProcessor>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();