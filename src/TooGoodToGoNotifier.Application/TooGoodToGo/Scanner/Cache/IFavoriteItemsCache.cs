namespace TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache; 

public interface IFavoriteItemsCache {
    Task<Dictionary<string, FavoriteItemDto>> Get();
    Task Persist(Dictionary<string, FavoriteItemDto> favoriteItems);
}