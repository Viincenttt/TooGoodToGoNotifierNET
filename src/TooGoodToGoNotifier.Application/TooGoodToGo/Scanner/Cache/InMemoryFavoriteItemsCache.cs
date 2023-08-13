namespace TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache; 

public class InMemoryFavoriteItemsCache : IFavoriteItemsCache {
    private readonly Dictionary<string, FavoriteItemDto> _favoriteItems = new();

    public Task<Dictionary<string, FavoriteItemDto>> Get() {
        return Task.FromResult(_favoriteItems);
    }

    public Task Persist(Dictionary<string, FavoriteItemDto> favoriteItems) {
        _favoriteItems.Clear();
        foreach (var item in favoriteItems) {
            _favoriteItems.Add(item.Key, item.Value);
        }

        return Task.CompletedTask;
    }
}