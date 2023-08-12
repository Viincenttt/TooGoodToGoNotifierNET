using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

public record FavoritesItemsResponse {
    [JsonProperty("items")]
    public required IEnumerable<ItemResponse> Items { get; init; }

    public record ItemResponse {
        [JsonProperty("display_name")]
        public required string DisplayName { get; init; }
    
        [JsonProperty("items_available")]
        public required int ItemsAvailable { get; init; }
    }
}