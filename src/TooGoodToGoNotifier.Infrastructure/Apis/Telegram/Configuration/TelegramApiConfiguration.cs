namespace TooGoodToGoNotifier.Infrastructure.Apis.Telegram.Configuration; 

public record TelegramApiConfiguration {
    public required string BotToken { get; init; }
    public required string ChatId { get; init; }
}