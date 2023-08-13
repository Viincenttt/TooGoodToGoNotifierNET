using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Domain.Configuration;

namespace TooGoodToGoNotifier.Presentation.Console.Services; 

public class TimedFavoritesScanner : BackgroundService {
    private readonly TimeSpan _timeBetweenScanning = TimeSpan.FromSeconds(30);
    
    private readonly FavoritesScanner _favoritesScanner;
    private readonly TooGoodToGoConfiguration _options;

    public TimedFavoritesScanner(FavoritesScanner favoritesScanner, IOptions<TooGoodToGoConfiguration> options) {
        _favoritesScanner = favoritesScanner;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        while (!cancellationToken.IsCancellationRequested) {
            await _favoritesScanner.ScanFavorites(_options.Email);
            await Task.Delay(_timeBetweenScanning, cancellationToken);
        }
    }
}