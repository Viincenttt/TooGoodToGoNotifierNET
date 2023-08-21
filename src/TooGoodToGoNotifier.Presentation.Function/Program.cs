using Microsoft.Extensions.Hosting;
using TooGoodToGoNotifier.Application;
using TooGoodToGoNotifier.Infrastructure;
using TooGoodToGoNotifier.Presentation.Function;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services)  =>
    {
        services.AddInfrastructureServices(hostContext.Configuration);
        services.AddApplicationServices();
        services.AddFunctionAppServices(hostContext.Configuration);
    })
    .Build();

await host.RunAsync();