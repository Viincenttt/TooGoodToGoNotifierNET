using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

public record AuthenticateByPollingIdResponse {
    [JsonProperty("access_token")]
    public required string AccessToken { get; init; }
    
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; init; }
    
    [JsonProperty("access_token_ttl_seconds")]
    public required int AccessTokenTtlSeconds { get; init; }
    
    [JsonProperty("startup_data")]
    public required StartupDataResponse StartupData { get; init; }
    
    public record StartupDataResponse {
        [JsonProperty("user")]
        public required UserResponse User { get; set; }
    }
    
    public record UserResponse {
        [JsonProperty("user_id")]
        public required string UserId { get; set; }
    }
}