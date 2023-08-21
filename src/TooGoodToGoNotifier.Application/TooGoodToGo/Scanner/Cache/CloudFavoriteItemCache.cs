using Newtonsoft.Json;
using TooGoodToGoNotifier.Application.Common.Interfaces;

namespace TooGoodToGoNotifier.Application.TooGoodToGo.Scanner.Cache; 

public class CloudFavoriteItemCache : IFavoriteItemsCache {
    private readonly ICloudKeyValueCacheProvider _cloudKeyValueCacheProvider;
    private const string FavoriteItemsCacheKey = "favoriteitems";

    public CloudFavoriteItemCache(ICloudKeyValueCacheProvider cloudKeyValueCacheProvider) {
        _cloudKeyValueCacheProvider = cloudKeyValueCacheProvider;
    }

    public async Task<Dictionary<string, FavoriteItemDto>> Get() {
        string? cachedValue = await _cloudKeyValueCacheProvider.Get(FavoriteItemsCacheKey);
        if (cachedValue == null) {
            return new Dictionary<string, FavoriteItemDto>();
        }

        var items = JsonConvert.DeserializeObject<IEnumerable<FavoriteItemDto>>(cachedValue);
        return items!.ToDictionary(x => x.ItemId, x => x);
    }

    public async Task Persist(Dictionary<string, FavoriteItemDto> favoriteItems) {
        var timeToCache = TimeSpan.FromMinutes(15);
        string cachedValue = JsonConvert.SerializeObject(favoriteItems.Values);
        await _cloudKeyValueCacheProvider.Save(FavoriteItemsCacheKey, cachedValue, timeToCache);
    }
}