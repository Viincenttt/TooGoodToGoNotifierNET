using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Domain.Configuration;

namespace TooGoodToGoNotifier.Presentation.Function.Functions; 

public class FavoritesScannerFunction {
    private readonly FavoritesScanner _favoritesScanner;
    private readonly TooGoodToGoConfiguration _options;

    public FavoritesScannerFunction(FavoritesScanner favoritesScanner, IOptions<TooGoodToGoConfiguration> options) {
        _favoritesScanner = favoritesScanner;
        _options = options.Value;
    }

    [Function("FavoritesScanner")]
    [ExponentialBackoffRetry(3, "00:00:45", "00:03:00")]
    public async Task RunAsync([TimerTrigger("%FavoritesScannerTriggerTime%")] TimerInfo myTimer) {
        await _favoritesScanner.ScanFavorites(_options.Email);
    }
}