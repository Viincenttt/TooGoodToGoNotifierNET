using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TooGoodToGoNotifier.Infrastructure.Exceptions;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;

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

    public async Task<FavoritesItemsResponse> GetFavoritesItems(string bearerToken, FavoritesItemsRequest request) {
        return await PostAsync<FavoritesItemsResponse>($"item/v8/", request, bearerToken)
            .ConfigureAwait(false);
    }
    
    private async Task<T> SendHttpRequest<T>(HttpMethod httpMethod, string relativeUri, object? data = null, string? bearerToken = null) {
        HttpRequestMessage httpRequest = CreateHttpRequest(httpMethod, relativeUri, data, bearerToken);
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
    
    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUri, object? data, string? bearerToken = null) {
        HttpRequestMessage httpRequest = new HttpRequestMessage(method, new Uri(new Uri(ApiBaseUrl), relativeUri));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.TryAddWithoutValidation("user-agent", "TGTG/23.7.12 Dalvik/2.1.0 (Linux; U; Android 9; AFTKA Build/PS7285.2877N");
        httpRequest.Headers.Add("accept-language", "en-UK");
        httpRequest.Headers.Add("accept", "application/json");
        if (!string.IsNullOrEmpty(bearerToken)) {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        if (data != null) {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            httpRequest.Content = content;
        }

        return httpRequest;
    }
    
    private async Task<T> PostAsync<T>(string relativeUri, object data, string? bearerToken = null) {
        return await SendHttpRequest<T>(HttpMethod.Post, relativeUri, data, bearerToken).ConfigureAwait(false);
    }
}