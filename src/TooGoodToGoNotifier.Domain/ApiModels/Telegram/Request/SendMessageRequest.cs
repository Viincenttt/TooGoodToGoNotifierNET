namespace TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request; 

public record SendMessageRequest {
    public required string Message { get; init; }
}