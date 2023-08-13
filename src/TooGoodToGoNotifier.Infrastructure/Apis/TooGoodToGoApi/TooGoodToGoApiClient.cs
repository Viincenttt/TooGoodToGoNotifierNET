using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;
using TooGoodToGoNotifier.Domain.Exceptions;

namespace TooGoodToGoNotifier.Infrastructure.Apis.TooGoodToGoApi;

public class TooGoodToGoApiClient : ITooGoodToGoApiClient {
    private const string ApiBaseUrl = "https://apptoogoodtogo.com/api/";

    private readonly HttpClient _httpClient;

    public TooGoodToGoApiClient(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<AuthenticateByEmailResponse> AuthenticateByEmail(AuthenticateByEmailRequest request) {
        return await PostAsync<AuthenticateByEmailResponse>($"auth/v3/authByEmail", request)
            .ConfigureAwait(false);
    }

    public async Task<AuthenticateByPollingIdResponse?> AuthenticateByPollingId(AuthenticateByPollingIdRequest request) {
        return await PostAsync<AuthenticateByPollingIdResponse?>($"auth/v3/authByRequestPollingId", request)
            .ConfigureAwait(false);
    }

    public async Task<RefreshAccessTokenResponse> RefreshAccessToken(RefreshAccessTokenRequest request) {
        return await PostAsync<RefreshAccessTokenResponse>($"auth/v3/token/refresh", request)
            .ConfigureAwait(false);
    }

    public async Task<FavoritesItemsResponse> GetFavoritesItems(string accessToken, FavoritesItemsRequest request) {
        return await PostAsync<FavoritesItemsResponse>($"item/v8/", request, accessToken)
            .ConfigureAwait(false);
    }
    
    private async Task<T> SendHttpRequest<T>(HttpMethod httpMethod, string relativeUri, object? data = null, string? accessToken = null) {
        HttpRequestMessage httpRequest = CreateHttpRequest(httpMethod, relativeUri, data, accessToken);
        if (data != null) {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            httpRequest.Content = content;
        }

        var response = await _httpClient.SendAsync(httpRequest).ConfigureAwait(false);
        return await ProcessHttpResponseMessage<T>(response).ConfigureAwait(false);
    }
    
    private async Task<T> ProcessHttpResponseMessage<T>(HttpResponseMessage response) {
        var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.IsSuccessStatusCode) {
            return JsonConvert.DeserializeObject<T>(resultContent)!;
        }

        throw new TooGoodToGoApiException(response.StatusCode, resultContent);
    }
    
    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUri, object? data, string? accessToken = null) {
        HttpRequestMessage httpRequest = new HttpRequestMessage(method, new Uri(new Uri(ApiBaseUrl), relativeUri));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.TryAddWithoutValidation("user-agent", "TGTG/23.7.12 Dalvik/2.1.0 (Linux; U; Android 9; AFTKA Build/PS7285.2877N");
        httpRequest.Headers.Add("accept-language", "en-UK");
        if (!string.IsNullOrEmpty(accessToken)) {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        if (data != null) {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            httpRequest.Content = content;
        }

        return httpRequest;
    }
    
    private async Task<T> PostAsync<T>(string relativeUri, object data, string? accessToken = null) {
        return await SendHttpRequest<T>(HttpMethod.Post, relativeUri, data, accessToken).ConfigureAwait(false);
    }
}