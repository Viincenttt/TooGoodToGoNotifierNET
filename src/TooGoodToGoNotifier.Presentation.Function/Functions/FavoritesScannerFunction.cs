using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;

namespace TooGoodToGoNotifier.Presentation.Function.Functions; 

public class FavoritesScannerFunction {
    private readonly SecretClient _secretClient;

    public FavoritesScannerFunction(SecretClient secretClient) {
        _secretClient = secretClient;
    }

    [Function("FavoritesScanner")]
    public async Task RunAsync([TimerTrigger("*/1 * * * *")] TimerInfo myTimer) {
        var secret = _secretClient.GetSecret("mysecret");
    }
}