using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models; 

public record GpsCoordinates {
    [JsonProperty("longitude")]
    public decimal Longitude { get; init; } = 0.0m;
    
    [JsonProperty("latitude")]
    public decimal Latitude { get; init; } = 0.0m;
}