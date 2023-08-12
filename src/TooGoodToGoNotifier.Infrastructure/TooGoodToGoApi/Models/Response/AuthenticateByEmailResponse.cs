using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response; 

public record AuthenticateByEmailResponse {
    [JsonProperty("polling_id")]
    public required string PollingId { get; init; } 
}