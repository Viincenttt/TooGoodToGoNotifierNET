using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram.Configuration;
using TooGoodToGoNotifier.Infrastructure.Apis.TooGoodToGoApi;
using TooGoodToGoNotifier.Infrastructure.Azure;
using TooGoodToGoNotifier.Infrastructure.Azure.Configuration;
using TooGoodToGoNotifier.Infrastructure.Time;

namespace TooGoodToGoNotifier.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration) {
        services.AddTransient<IDateTimeProvider, SystemDateTime>();
        services.AddTransient<ISecretsManager, KeyVaultSecretManager>();

        services.AddHttpClient<ITooGoodToGoApiClient, TooGoodToGoApiClient>();
        services.AddHttpClient<ITelegramApiClient, TelegramApiClient>();

        services.AddOptions<TelegramApiConfiguration>().Bind(configuration.GetSection("Notifications:Telegram"));
        services.AddOptions<BlobStorageCacheConfiguration>().Bind(configuration.GetSection("BlobStorageCache"));

        return services;
    }

    public static IServiceCollection AddKeyvault(
        this IServiceCollection services, 
        IConfiguration configuration) {
        services.AddAzureClients(clientBuilder => {
            string keyVaultUri = configuration["KeyvaultUri"]!;
            clientBuilder.AddSecretClient(new Uri(keyVaultUri));
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });

        return services;
    }

    public static IServiceCollection AddBlobServiceStorage(this IServiceCollection services, IConfiguration configuration) {
        services.AddTransient<ICloudKeyValueCacheProvider, BlobStorageCacheProvider>();

        services.AddAzureClients(clientBuilder => {
            clientBuilder.AddBlobServiceClient(new Uri(configuration.GetValue<string>("BlobStorageCache:Uri")!));
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });

        return services;
    }
}