using Moq;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.Notifications;
using TooGoodToGoNotifier.Application.TooGoodToGo.Scanner;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request;

namespace TooGoodToGoNotifier.Application.Tests.Notifications; 

public class TelegramNotifierTests {
    [Fact]
    public async Task Notify_invokes_telegramclient_send_message() {
        // Arrange
        var telegramApiClient = new Mock<ITelegramApiClient>();
        var favoriteItem = new FavoriteItemDto {
            ItemId = "item-id",
            DisplayName = "my-favorite-item",
            ItemsAvailable = 2
        };
        var sut = Sut(telegramApiClient);
        
        // Act
        await sut.Notify(favoriteItem);

        // Assert
        string expectedMessage = $"New availability for Store={favoriteItem.DisplayName}" +
                                 $" - Number of available items={favoriteItem.ItemsAvailable}";
        var expectedRequest = new SendMessageRequest {
            Message = expectedMessage
        };
        telegramApiClient.Verify(x => x.SendMessage(expectedRequest), Times.Once);
    }

    private static TelegramNotifier Sut(Mock<ITelegramApiClient>? telegramApiClient = null) {
        telegramApiClient ??= new Mock<ITelegramApiClient>();
        return new TelegramNotifier(telegramApiClient.Object);
    }
}