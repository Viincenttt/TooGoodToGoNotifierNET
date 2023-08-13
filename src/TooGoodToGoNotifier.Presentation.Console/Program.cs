using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
    services.AddHttpClient<ITooGoodToGoApiClient, TooGoodToGoApiClient>());

IHost host = builder.Build();

var client = host.Services.GetRequiredService<ITooGoodToGoApiClient>();
const string emailAddress = "vincentkok@live.nl";
var authenticateByEmailResult = await client.AuthenticateByEmail(new AuthenticateByEmailRequest {
    Email = emailAddress
});

await Task.Delay(TimeSpan.FromSeconds(10));

while (true) {
    var authenticateByPollingIdResult = await client.AuthenticateByPollingId(new AuthenticateByPollingIdRequest {
        Email = emailAddress,
        RequestPollingId = authenticateByEmailResult.PollingId
    });

    if (authenticateByPollingIdResult != null) {
        var accessTokenResponse = await client.RefreshAccessToken(new RefreshAccessTokenRequest {
            RefreshToken = authenticateByPollingIdResult.RefreshToken
        });

        /*
        var favorites = await client.GetFavoritesItems(
            bearerToken: authenticateByPollingIdResult.AccessToken, 
            request: new FavoritesItemsRequest {
                UserId = authenticateByPollingIdResult.StartupData.User.UserId
            }
        );*/
    }

    await Task.Delay(TimeSpan.FromSeconds(10));
}

host.Run();
