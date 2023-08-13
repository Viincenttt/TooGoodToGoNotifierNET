using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram;
using TooGoodToGoNotifier.Domain.ApiModels.Telegram.Request;
using TooGoodToGoNotifier.Domain.Exceptions;
using TooGoodToGoNotifier.Infrastructure.Apis.Telegram.Configuration;

namespace TooGoodToGoNotifier.Infrastructure.Apis.Telegram; 

public class TelegramApiClient : ITelegramApiClient {
    private const string ApiBaseUrl = "https://api.telegram.org/";
    
    private readonly TelegramApiConfiguration _options;
    private readonly HttpClient _httpClient;

    public TelegramApiClient(IOptions<TelegramApiConfiguration> options, HttpClient httpClient) {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<SendMessageResponse> SendMessage(SendMessageRequest request) {
        string relativeUri = $"bot{Uri.EscapeDataString(_options.BotToken)}/" +
                             $"sendMessage?chat_id={_options.ChatId}&" +
                             $"text={Uri.EscapeDataString(request.Message)}";
        
        return await GetAsync<SendMessageResponse>(relativeUri)
            .ConfigureAwait(false);
    }

    private async Task<T> SendHttpRequest<T>(HttpMethod httpMethod, string relativeUri) {
        HttpRequestMessage httpRequest = CreateHttpRequest(httpMethod, relativeUri);
        var response = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
        return await ProcessHttpResponseMessage<T>(response).ConfigureAwait(false);
    }
    
    private async Task<T> ProcessHttpResponseMessage<T>(HttpResponseMessage response) {
        var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.IsSuccessStatusCode) {
            return JsonConvert.DeserializeObject<T>(resultContent)!;
        }

        throw new TelegramApiException(response.StatusCode, resultContent);
    }

    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUri) {
        Uri uri = new Uri(new Uri(ApiBaseUrl), relativeUri);
        HttpRequestMessage httpRequest = new HttpRequestMessage(method, uri);
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


        return httpRequest;
    }
    
    private async Task<T> GetAsync<T>(string relativeUri) {
        return await SendHttpRequest<T>(HttpMethod.Get, relativeUri).ConfigureAwait(false);
    }
}