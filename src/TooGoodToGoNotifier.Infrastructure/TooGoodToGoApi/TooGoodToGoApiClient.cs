using System.Net;
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
        return await this.PostAsync<AuthenticateByEmailResponse>($"auth/v3/authByEmail", request)
            .ConfigureAwait(false);
    }

    public async Task<AuthenticateByPollingIdResponse> AuthenticateByPollingId(AuthenticateByPollingIdRequest request) {
        return await this.PostAsync<AuthenticateByPollingIdResponse>($"auth/v3/authByRequestPollingId", request)
            .ConfigureAwait(false);
    }
    
    private async Task<T> SendHttpRequest<T>(HttpMethod httpMethod, string relativeUri, object? data = null) {
        HttpRequestMessage httpRequest = this.CreateHttpRequest(httpMethod, relativeUri);
        if (data != null) {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            httpRequest.Content = content;
        }

        var response = await this._httpClient.SendAsync(httpRequest).ConfigureAwait(false);
        return await this.ProcessHttpResponseMessage<T>(response).ConfigureAwait(false);
    }
    
    private async Task<T> ProcessHttpResponseMessage<T>(HttpResponseMessage response) {
        var resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.IsSuccessStatusCode) {
            return JsonConvert.DeserializeObject<T>(resultContent)!;
        }

        switch (response.StatusCode) {
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.UnsupportedMediaType:
            case HttpStatusCode.TooManyRequests:
                throw new TooGoodToGoApiException(response.StatusCode, resultContent);
            default:
                throw new HttpRequestException(
                    $"Unknown http exception occured with status code: {(int) response.StatusCode}.");
        }
    }
    
    private HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUri, HttpContent? content = null) {
        HttpRequestMessage httpRequest = new HttpRequestMessage(method, new Uri(new Uri(ApiBaseUrl), relativeUri));
        httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequest.Headers.TryAddWithoutValidation("user-agent", "TGTG/23.7.12 Dalvik/2.1.0 (Linux; U; Android 9; AFTKA Build/PS7285.2877N");
        httpRequest.Headers.Add("accept-language", "en-UK");
        httpRequest.Headers.Add("accept", "application/json");
        httpRequest.Content = content;

        return httpRequest;
    }
    
    private async Task<T> PostAsync<T>(string relativeUri, object data) {
        return await this.SendHttpRequest<T>(HttpMethod.Post, relativeUri, data).ConfigureAwait(false);
    }
    
    protected async Task<T> GetAsync<T>(string relativeUri) {
        return await this.SendHttpRequest<T>(HttpMethod.Get, relativeUri).ConfigureAwait(false);
    }
}