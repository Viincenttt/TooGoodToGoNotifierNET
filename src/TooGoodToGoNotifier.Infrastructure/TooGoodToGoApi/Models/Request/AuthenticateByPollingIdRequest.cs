using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request; 

public record AuthenticateByPollingIdRequest {
    [JsonProperty("device_type")]
    public string DeviceType { get; init; } = "ANDROID";
    
    [JsonProperty("email")]
    public required string Email { get; init; }
    
    [JsonProperty("request_polling_id")]
    public required string RequestPollingId { get; init; }
}