using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request;

namespace TooGoodToGoNotifier.Application.Notifications; 

public class TelegramNotifier : INotifier {
    private readonly ITelegramApiClient _telegramApiClient;

    public TelegramNotifier(ITelegramApiClient telegramApiClient) {
        _telegramApiClient = telegramApiClient;
    }

    public async Task Notify(FavoriteItemDto favoriteItemDto) {
        string message = $"New availability for Store={favoriteItemDto.DisplayName}" +
                         $" - Number of available items={favoriteItemDto.ItemsAvailable}";
        var request = new SendMessageRequest {
            Message = message
        };
        await _telegramApiClient.SendMessage(request);
    }
}