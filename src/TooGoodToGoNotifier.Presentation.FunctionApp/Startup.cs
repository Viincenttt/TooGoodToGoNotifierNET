using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using TooGoodToGoNotifier.Presentation.FunctionApp;

[assembly: FunctionsStartup(typeof(Startup))]

namespace TooGoodToGoNotifier.Presentation.FunctionApp; 

public class Startup : FunctionsStartup {
    public override void Configure(IFunctionsHostBuilder builder) {
        builder.Services.AddAzureClients(clientBuilder => {
            string keyVaultUri = Environment.GetEnvironmentVariable("KeyvaultUri")!;
            clientBuilder.AddSecretClient(new Uri(keyVaultUri));
        });
    }
}