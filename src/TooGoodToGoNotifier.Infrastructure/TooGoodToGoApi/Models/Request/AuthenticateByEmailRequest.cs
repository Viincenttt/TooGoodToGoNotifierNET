using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request; 

public record AuthenticateByEmailRequest {
    [JsonProperty("device_type")]
    public string DeviceType { get; init; } = "ANDROID";
    
    [JsonProperty("email")]
    public required string Email { get; init; }
}