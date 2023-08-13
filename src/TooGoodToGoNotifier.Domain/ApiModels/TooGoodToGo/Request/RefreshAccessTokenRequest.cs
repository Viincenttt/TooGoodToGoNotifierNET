using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request; 

public record RefreshAccessTokenRequest {
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }
}