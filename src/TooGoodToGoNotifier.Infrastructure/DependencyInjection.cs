using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram.Configuration;
using TooGoodToGoNotifier.Infrastructure.Apis.TooGoodToGoApi;

namespace TooGoodToGoNotifier.Infrastructure; 

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddHttpClient<ITooGoodToGoApiClient, TooGoodToGoApiClient>();
        services.AddHttpClient<ITelegramApiClient, TelegramApiClient>();

        services.AddOptions<TelegramApiConfiguration>().Bind(configuration.GetSection("Notifications:Telegram"));
        
        return services;
    }
}