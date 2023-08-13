using Newtonsoft.Json;

namespace TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response; 

public record AuthenticateByEmailResponse {
    [JsonProperty("polling_id")]
    public required string PollingId { get; init; } 
}