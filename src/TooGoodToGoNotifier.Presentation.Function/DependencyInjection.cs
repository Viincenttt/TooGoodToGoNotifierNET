using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application;
using TooGoodToGoNotifier.Domain.Configuration;
using TooGoodToGoNotifier.Infrastructure;

namespace TooGoodToGoNotifier.Presentation.Function; 

public static class DependencyInjection
{
    public static IServiceCollection AddFunctionAppServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddCloudBasedCache();
        services.AddKeyvault(configuration);
        services.AddBlobServiceStorage(configuration);
        
        services.AddOptions<TooGoodToGoConfiguration>().Bind(configuration.GetSection("TooGoodToGo"));

        return services;
    }
}