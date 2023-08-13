using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddAzureClients(clientBuilder => {
            string keyVaultUri = Environment.GetEnvironmentVariable("KeyvaultUri")!;
            clientBuilder.AddSecretClient(new Uri(keyVaultUri));
        });
    })
    .Build();

host.Run();