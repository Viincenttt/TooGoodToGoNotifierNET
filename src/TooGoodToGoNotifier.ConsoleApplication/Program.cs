using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
    services.AddHttpClient<ITooGoodToGoApiClient, TooGoodToGoApiClient>());

IHost host = builder.Build();

var client = host.Services.GetRequiredService<ITooGoodToGoApiClient>();
var result = await client.AuthenticateByEmail(new AuthenticateByEmailRequest {
    Email = "vincentkok@live.nl"
});

host.Run();
