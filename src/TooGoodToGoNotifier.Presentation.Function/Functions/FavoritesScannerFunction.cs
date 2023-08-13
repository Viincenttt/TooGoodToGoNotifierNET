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
    public async Task RunAsync([TimerTrigger("*/1 * * * *")] TimerInfo myTimer) {
        await _favoritesScanner.ScanFavorites(_options.Email);
    }
}