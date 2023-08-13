using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Domain.Configuration;

namespace TooGoodToGoNotifier.Presentation.Console.Services; 

public class TimedFavoritesScanner : BackgroundService {
    private readonly TimeSpan _timeBetweenScanning = TimeSpan.FromSeconds(30);
    
    private readonly FavoritesScanner _favoritesScanner;
    private readonly TooGoodToGoConfiguration _options;
    private readonly ILogger<TimedFavoritesScanner> _logger;

    public TimedFavoritesScanner(
        FavoritesScanner favoritesScanner, 
        IOptions<TooGoodToGoConfiguration> options, 
        ILogger<TimedFavoritesScanner> logger) {
        
        _favoritesScanner = favoritesScanner;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Started scanning continuously for email={email}", 
            _options.Email);
        
        while (!cancellationToken.IsCancellationRequested) {
            _logger.LogInformation("Starting scanning for favorites");

            try {
                await _favoritesScanner.ScanFavorites(_options.Email, cancellationToken);
            }
            catch (Exception e) {
                _logger.LogError("Error while scanning favorites Error={error}. Retrying in {timeBetweenScanning} seconds",
                    e.Message,
                    _timeBetweenScanning.TotalSeconds);
            }

            _logger.LogInformation("Scan complete, waiting for {timeBetweenScanning} seconds", 
                _timeBetweenScanning.TotalSeconds);
            
            await Task.Delay(_timeBetweenScanning, cancellationToken);
        }
    }
}