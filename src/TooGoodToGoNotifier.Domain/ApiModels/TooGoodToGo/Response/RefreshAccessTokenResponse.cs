using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

public record RefreshAccessTokenResponse {
    [JsonProperty("access_token")]
    public required string AccessToken { get; init; }
    
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; init; }
    
    [JsonProperty("access_token_ttl_seconds")]
    public required int AccessTokenTtlSeconds { get; init; }
}