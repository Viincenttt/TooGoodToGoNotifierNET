using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request; 

public record FavoritesItemsRequest {
    [JsonProperty("user_id")]
    public required string UserId { get; init; }

    [JsonProperty("origin")] 
    public GpsCoordinates Origin { get; init; } = new GpsCoordinates();

    [JsonProperty("radius")]
    public int Radius { get; init; } = 1;

    [JsonProperty("page_size")] 
    public int PageSize { get; init; } = 400;
    
    [JsonProperty("page")]
    public int Page { get; init; } = 1;

    [JsonProperty("discover")] 
    public bool Discover { get; init; } = false;

    [JsonProperty("favorites_only")] 
    public bool FavoritesOnly { get; init; } = true;
    
    [JsonProperty("item_categories")] 
    public object[] ItemCategories { get; init; } = Array.Empty<object>();
    
    [JsonProperty("diet_categories")] 
    public object[] DietCategories { get; init; } = Array.Empty<object>();
    
    [JsonProperty("pickup_earliest")] 
    public object? PickupEarliest { get; init; }
    
    [JsonProperty("pickup_latest")] 
    public object? PickupLatest { get; init; }
    
    [JsonProperty("search_phrase")] 
    public string? SearchPhrase { get; init; }
    
    [JsonProperty("with_stock_only")] 
    public bool WithStockOnly { get; init; } = false;
    
    [JsonProperty("hidden_only")] 
    public bool HiddenOnly { get; init; } = false;
    
    [JsonProperty("we_care_only")] 
    public bool WeCareOnly { get; init; } = false;
}