using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;
using TooGoodToGoNotifier.Application;
using TooGoodToGoNotifier.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services)  =>
    {
        services.AddInfrastructureServices(hostContext.Configuration);
        services.AddSecretManagerAuthenticationCache();
        services.AddApplicationServices();
        
        services.AddAzureClients(clientBuilder => {
            string keyVaultUri = Environment.GetEnvironmentVariable("KeyvaultUri")!;
            clientBuilder.AddSecretClient(new Uri(keyVaultUri));
        });
    })
    .Build();

await host.RunAsync();