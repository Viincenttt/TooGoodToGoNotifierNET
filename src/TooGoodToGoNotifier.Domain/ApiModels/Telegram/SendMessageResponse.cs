namespace TooGoodToGoNotifier.Domain.ApiModels.Telegram; 

public record SendMessageResponse {
    public required bool Ok { get; init; }
}