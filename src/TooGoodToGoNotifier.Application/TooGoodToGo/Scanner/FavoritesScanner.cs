using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

namespace TooGoodToGoNotifier.Application.TooGoodToGo.Scanner; 

public class FavoritesScanner {
    private readonly TooGoodToGoAuthenticator _tooGoodToGoAuthenticator;
    private readonly ITooGoodToGoApiClient _tooGoodToGoApiClient;
    private readonly IFavoriteItemsCache _favoriteItemsCache;
    private readonly IEnumerable<INotifier> _notifiers;

    public FavoritesScanner(
        TooGoodToGoAuthenticator tooGoodToGoAuthenticator, 
        ITooGoodToGoApiClient tooGoodToGoApiClient, 
        IFavoriteItemsCache favoriteItemsCache, 
        IEnumerable<INotifier> notifiers) {
        
        _tooGoodToGoAuthenticator = tooGoodToGoAuthenticator;
        _tooGoodToGoApiClient = tooGoodToGoApiClient;
        _favoriteItemsCache = favoriteItemsCache;
        _notifiers = notifiers;
    }
    
    public async Task ScanFavorites(string email) {
        AuthenticationDto authentication = await _tooGoodToGoAuthenticator.Authenticate(email);
        Dictionary<string, FavoriteItemDto> previousFavorites = await _favoriteItemsCache.Get();
        Dictionary<string, FavoriteItemDto> newFavorites = await GetFavoritesFromApi(authentication);

        foreach (var newFavorite in newFavorites) {
            if (newFavorite.Value.ItemsAvailable > 0) {
                previousFavorites.TryGetValue(newFavorite.Key, out FavoriteItemDto? previousFavorite);
                if (previousFavorite == null || previousFavorite.ItemsAvailable == 0) {
                    await NotifyNewAvailability(newFavorite.Value);
                }
            }
        }

        await _favoriteItemsCache.Persist(newFavorites);
    }

    private async Task NotifyNewAvailability(FavoriteItemDto favoriteItem) {
        foreach (INotifier notifier in _notifiers) {
            try {
                await notifier.Notify(favoriteItem);
            }
            catch (Exception e) {
                // TODO: Log error
            }
        }
    }

    private async Task<Dictionary<string, FavoriteItemDto>> GetFavoritesFromApi(AuthenticationDto authentication) {
        FavoritesItemsResponse response = await _tooGoodToGoApiClient.GetFavoritesItems(authentication.AccessToken, new FavoritesItemsRequest {
            UserId = authentication.UserId
        });

        return response.Items.ToDictionary(x => x.Item.ItemId, MapToFavoriteItemDto);
    }

    private FavoriteItemDto MapToFavoriteItemDto(FavoritesItemsResponse.ItemResponse item) {
        return new FavoriteItemDto {
            ItemId = item.Item.ItemId,
            DisplayName = item.DisplayName,
            ItemsAvailable = item.ItemsAvailable
        };
    }
}