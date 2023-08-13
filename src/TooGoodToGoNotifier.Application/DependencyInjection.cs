using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache;

namespace TooGoodToGoNotifier.Application; 

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services) {
        services.AddTransient<IAuthenticationCache, InMemoryAuthenticationCache>();
        services.AddTransient<TooGoodToGoAuthenticator>();
        
        return services;
    }
}