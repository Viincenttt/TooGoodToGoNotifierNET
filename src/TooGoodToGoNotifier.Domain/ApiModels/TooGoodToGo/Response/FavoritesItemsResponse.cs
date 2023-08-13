using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

public record FavoritesItemsResponse {
    [JsonProperty("items")]
    public required IEnumerable<ItemResponse> Items { get; init; }

    public record ItemResponse {
        [JsonProperty("item")]
        public required ItemDetails Item { get; set; }

        [JsonProperty("display_name")]
        public required string DisplayName { get; init; }
    
        [JsonProperty("items_available")]
        public required int ItemsAvailable { get; init; }
        
        public record ItemDetails {
            [JsonProperty("item_id")]
            public required string ItemId { get; set; }
        }
    }
}