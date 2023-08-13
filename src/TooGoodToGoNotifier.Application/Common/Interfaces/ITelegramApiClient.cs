using TooGoodToGoNotifier.Domain.ApiModels.Telegram;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request;

namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface ITelegramApiClient {
    Task<SendMessageResponse> SendMessage(SendMessageRequest request);
}