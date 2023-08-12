using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request; 

public record RefreshAccessTokenRequest {
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }
}