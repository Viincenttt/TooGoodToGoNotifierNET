using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TooGoodToGoNotifier.Presentation.FunctionApp.Functions; 

public class FavoritesScannerFunction {
    private readonly SecretClient _secretClient;

    public FavoritesScannerFunction(SecretClient secretClient) {
        _secretClient = secretClient;
    }

    [FunctionName("FavoritesScanner")]
    public async Task RunAsync([TimerTrigger("*/1 * * * *")] TimerInfo myTimer, ILogger log) {
        var secret = _secretClient.GetSecret("mysecret");
        log.LogInformation("Retrieved secret from keyvault: {secret}", secret.Value.Value);
    }
}