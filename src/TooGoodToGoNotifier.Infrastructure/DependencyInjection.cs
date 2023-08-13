using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;

namespace TooGoodToGoNotifier.Infrastructure; 

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services) {
        services.AddHttpClient<ITooGoodToGoApiClient, TooGoodToGoApiClient>();
        
        return services;
    }
}