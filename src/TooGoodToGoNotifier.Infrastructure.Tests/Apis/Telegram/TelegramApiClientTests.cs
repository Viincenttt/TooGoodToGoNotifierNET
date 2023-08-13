using FluentAssertions;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram.Configuration;

namespace TooGoodToGoNotifier.Infrastructure.Tests.Apis.Telegram; 

public class TelegramApiClientTests {
    private const string ApiBaseUrl = "https://api.telegram.org/";

    [Fact]
    public async Task SendMessage_sends_get_request_to_telegram_api() {
        // Arrange
        const string message = "this is my telegram message";
        const string chatId = "-12345";
        const string botToken = "my-bot-token";
        var options = CreateOptions(botToken);
        string expectedUrl = $"{ApiBaseUrl}bot{Uri.EscapeDataString(botToken)}/" +
                             $"sendMessage?chat_id={chatId}&" +
                             $"text={Uri.EscapeDataString(message)}";
        string jsonResponse = CreateMessageResponse(true);
        var mockHttp = CreateMockHttpMessageHandler(HttpMethod.Get, expectedUrl, jsonResponse);
        HttpClient httpClient = mockHttp.ToHttpClient();
        var telegramClient = new TelegramApiClient(options, httpClient);

        // Act
        SendMessageResponse response = await telegramClient.SendMessage(new SendMessageRequest {
            ChatId = chatId,
            Message = message
        });

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
        response.Ok.Should().BeTrue();
    }

    private string CreateMessageResponse(bool ok) {
        return @$"{{
            ""ok"": {ok.ToString().ToLowerInvariant()}
        }}"; 
    }

    private IOptions<TelegramApiConfiguration> CreateOptions(string botToken) {
        return Options.Create(new TelegramApiConfiguration {
            BotToken = botToken
        });
    }

    private MockHttpMessageHandler CreateMockHttpMessageHandler(HttpMethod httpMethod, string url, string response, string? expectedPartialContent = null) {
        MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
        MockedRequest mockedRequest = mockHttp.Expect(httpMethod, url)
            .Respond("application/json", response);

        if (!string.IsNullOrEmpty(expectedPartialContent)) {
            mockedRequest.WithPartialContent(expectedPartialContent);
        }

        return mockHttp;
    }
}