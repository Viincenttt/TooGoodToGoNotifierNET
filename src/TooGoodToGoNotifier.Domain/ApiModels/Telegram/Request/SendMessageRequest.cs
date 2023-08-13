namespace TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request; 

public record SendMessageRequest {
    public required string ChatId { get; init; }
    public required string Message { get; init; }
}