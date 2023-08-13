using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using TooGoodToGoNotifier.Application;
using TooGoodToGoNotifier.Infrastructure;
using TooGoodToGoNotifier.Presentation.Console;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddEnvironmentVariables();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddConsoleServices(builder.Configuration);

IHost host = builder.Build();

host.Run();
