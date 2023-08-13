using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace TooGoodToGoNotifier.Presentation.FunctionApp.Functions; 

public static class FavoritesScannerFunction {
    [FunctionName("FavoritesScanner")]
    public static async Task RunAsync([TimerTrigger("*/1 * * * *")] TimerInfo myTimer, ILogger log) {
        log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
    }
}