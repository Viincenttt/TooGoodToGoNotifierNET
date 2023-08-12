using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

public record AuthenticateByPollingIdResponse {
    [JsonProperty("access_token")]
    public required string AccessToken { get; init; }
    
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; init; }
    
    [JsonProperty("access_token_ttl_seconds")]
    public required string AccessTokenTtlSeconds { get; init; }
    
    // TODO: Nested obj
    public required string UserId { get; init; }
}