using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram.Configuration;
using TooGoodToGoNotifier.Infrastructure.Apis.TooGoodToGoApi;
using TooGoodToGoNotifier.Infrastructure.Azure;
using TooGoodToGoNotifier.Infrastructure.Time;

namespace TooGoodToGoNotifier.Infrastructure; 

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddTransient<IDateTimeProvider, SystemDateTime>();
        services.AddTransient<ISecretsManager, KeyVaultSecretManager>();
        services.AddTransient<ICloudKeyValueCacheProvider, AzureRedisCacheProvider>();
        services.AddSingleton<IConnectionMultiplexer>(cfg =>
            ConnectionMultiplexer.Connect(configuration["RedisConnectionString"]!));
        
        services.AddHttpClient<ITooGoodToGoApiClient, TooGoodToGoApiClient>();
        services.AddHttpClient<ITelegramApiClient, TelegramApiClient>();

        services.AddOptions<TelegramApiConfiguration>().Bind(configuration.GetSection("Notifications:Telegram"));
        
        return services;
    }
}