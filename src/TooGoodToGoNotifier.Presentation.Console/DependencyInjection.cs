using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TooGoodToGoNotifier.Application;
using TooGoodToGoNotifier.Domain.Configuration;
using TooGoodToGoNotifier.Presentation.Console.Services;

namespace TooGoodToGoNotifier.Presentation.Console; 

public static class DependencyInjection
{
    public static IServiceCollection AddConsoleServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddInMemoryCache();
        services.AddOptions<TooGoodToGoConfiguration>().Bind(configuration.GetSection("TooGoodToGo"));
        services.AddHostedService<TimedFavoritesScanner>();

        services.AddSerilog((context, config) => {
            config.ReadFrom.Configuration(configuration);
        });
        
        return services;
    }
}