using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.Notifications;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache;

namespace TooGoodToGoNotifier.Application; 

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        services.AddTransient<ITooGoodToGoAuthenticator, TooGoodToGoAuthenticator>();

        services.AddTransient<IFavoriteItemsCache, InMemoryFavoriteItemsCache>();
        services.AddTransient<FavoritesScanner>();
        
        services.Add(new ServiceDescriptor(typeof(INotifier), typeof(TelegramNotifier), ServiceLifetime.Transient));
        
        return services;
    }

    public static IServiceCollection AddInMemoryAuthenticationCache(this IServiceCollection services) {
        services.AddTransient<IAuthenticationCache, InMemoryAuthenticationCache>();

        return services;
    }

    public static IServiceCollection AddSecretManagerAuthenticationCache(this IServiceCollection services) {
        services.AddTransient<IAuthenticationCache, SecretsManagerAuthenticationCache>();

        return services;
    }
}