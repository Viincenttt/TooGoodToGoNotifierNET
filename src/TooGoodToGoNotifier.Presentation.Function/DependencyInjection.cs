using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application;
using TooGoodToGoNotifier.Domain.Configuration;

namespace TooGoodToGoNotifier.Presentation.Function; 

public static class DependencyInjection
{
    public static IServiceCollection AddFunctionAppServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddCloudBasedCache();
        services.AddOptions<TooGoodToGoConfiguration>().Bind(configuration.GetSection("TooGoodToGo"));
        
        services.AddAzureClients(clientBuilder => {
            string keyVaultUri = configuration["KeyvaultUri"]!;
            clientBuilder.AddSecretClient(new Uri(keyVaultUri));
        });
        
        return services;
    }
}