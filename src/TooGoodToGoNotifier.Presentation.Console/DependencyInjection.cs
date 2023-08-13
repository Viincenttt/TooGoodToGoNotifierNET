using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Domain.Configuration;
using TooGoodToGoNotifier.Presentation.Console.Services;

namespace TooGoodToGoNotifier.Presentation.Console; 

public static class DependencyInjection
{
    public static IServiceCollection AddConsoleServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddOptions<TooGoodToGoConfiguration>().Bind(configuration.GetSection("TooGoodToGo"));
        services.AddHostedService<TimedFavoritesScanner>();
        
        return services;
    }
}