using System.Net;
using FluentAssertions;
using RichardSzalay.MockHttp;
using TooGoodToGoNotifier.Infrastructure.Exceptions;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

namespace TooGoodToGoNotifier.Infrastructure.Tests;

public class TooGoodToGoApiClientTests {
    private const string ApiBaseUrl = "https://apptoogoodtogo.com/api/";
    
    [Fact]
    public async Task Authenticate_by_email_request_response_matches() {
        // Arrange
        const string pollingId = "my-polling-id";
        string jsonResponse = CreateAuthenticateByEmailJson(pollingId);
        string expectedUrl = $"{ApiBaseUrl}auth/v3/authByEmail";
        var mockHttp = CreateMockHttpMessageHandler(HttpMethod.Post, expectedUrl, jsonResponse);
        HttpClient httpClient = mockHttp.ToHttpClient();
        var tgtgClient = new TooGoodToGoApiClient(httpClient);
        
        // Act
        AuthenticateByEmailResponse response = await tgtgClient.AuthenticateByEmail(new AuthenticateByEmailRequest {
            Email = "my-email@microsoft.com"
        });

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
        response.PollingId.Should().Be(pollingId);
    }
    
    [Fact]
    public async Task Authenticate_by_pollingId_with_empty_result_returns_null() {
        // Arrange
        string jsonResponse = string.Empty;
        string expectedUrl = $"{ApiBaseUrl}auth/v3/authByRequestPollingId";
        var mockHttp = CreateMockHttpMessageHandler(HttpMethod.Post, expectedUrl, jsonResponse);
        HttpClient httpClient = mockHttp.ToHttpClient();
        var tgtgClient = new TooGoodToGoApiClient(httpClient);
        
        // Act
        AuthenticateByPollingIdResponse? response = await tgtgClient.AuthenticateByPollingId(new AuthenticateByPollingIdRequest {
            Email = "my-email@microsoft.com",
            RequestPollingId = "my-polling-id"
        });

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
        response.Should().BeNull();
    }
    
    [Fact]
    public async Task Authenticate_by_pollingId_with_json_result_request_response_matches() {
        // Arrange
        const string accessToken = "my-access-token";
        const string refreshToken = "my-refresh-token";
        const int accessTokenTtlSeconds = 1000;
        const string userId = "my-userid";
        string jsonResponse = CreateAuthenticateByPollingIdJson(accessToken, refreshToken, accessTokenTtlSeconds, userId);
        string expectedUrl = $"{ApiBaseUrl}auth/v3/authByRequestPollingId";
        var mockHttp = CreateMockHttpMessageHandler(HttpMethod.Post, expectedUrl, jsonResponse);
        HttpClient httpClient = mockHttp.ToHttpClient();
        var tgtgClient = new TooGoodToGoApiClient(httpClient);
        
        // Act
        AuthenticateByPollingIdResponse? response = await tgtgClient.AuthenticateByPollingId(new AuthenticateByPollingIdRequest {
            Email = "my-email@microsoft.com",
            RequestPollingId = "my-polling-id"
        });

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
        response.Should().NotBeNull();
        response!.AccessToken.Should().Be(accessToken);
        response.RefreshToken.Should().Be(refreshToken);
        response.AccessTokenTtlSeconds.Should().Be(accessTokenTtlSeconds);
        response.StartupData.User.UserId.Should().Be(userId);
    }
    
    [Fact]
    public async Task Refresh_access_token_request_response_matches() {
        // Arrange
        const string accessToken = "my-access-token";
        const string refreshToken = "my-refresh-token";
        const int accessTokenTtlSeconds = 1000;
        string jsonResponse = CreateRefreshTokenJson(accessToken, refreshToken, accessTokenTtlSeconds);
        string expectedUrl = $"{ApiBaseUrl}auth/v3/token/refresh";
        var mockHttp = CreateMockHttpMessageHandler(HttpMethod.Post, expectedUrl, jsonResponse);
        HttpClient httpClient = mockHttp.ToHttpClient();
        var tgtgClient = new TooGoodToGoApiClient(httpClient);
        
        // Act
        RefreshAccessTokenResponse response = await tgtgClient.RefreshAccessToken(new RefreshAccessTokenRequest {
            RefreshToken = "my-refresh-token"
        });

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
        response.Should().NotBeNull();
        response!.AccessToken.Should().Be(accessToken);
        response.RefreshToken.Should().Be(refreshToken);
        response.AccessTokenTtlSeconds.Should().Be(accessTokenTtlSeconds);
    } 
    
    [Fact]
    public async Task Get_favorites_items_request_response_matches() {
        // Arrange
        const string myBearerToken = "my-bearer-token";
        const string displayName = "my-favorite-store";
        const int itemsAvailable = 11;
        string jsonResponse = CreateFavoritesItemsResponse(displayName, itemsAvailable);
        string expectedUrl = $"{ApiBaseUrl}item/v8/";
        var mockHttp = CreateMockHttpMessageHandler(HttpMethod.Post, expectedUrl, jsonResponse);
        HttpClient httpClient = mockHttp.ToHttpClient();
        var tgtgClient = new TooGoodToGoApiClient(httpClient);
        
        // Act
        FavoritesItemsResponse response = await tgtgClient.GetFavoritesItems(myBearerToken, new FavoritesItemsRequest {
            UserId = "my-user-id"
        });

        // Assert
        mockHttp.VerifyNoOutstandingExpectation();
        response.Should().NotBeNull();
        response.Items.Should().HaveCount(1);
        FavoritesItemsResponse.ItemResponse item = response.Items.First();
        item.DisplayName.Should().Be(displayName);
        item.ItemsAvailable.Should().Be(itemsAvailable);
    }

    [Fact]
    public async Task Http_status_code_is_unsuccessful_TooGoodToGoApiException_is_thrown() {
        // Arrange
        const string errorMessage = "my-error-message";
        const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Post, $"{ApiBaseUrl}auth/v3/authByEmail")
            .Respond(statusCode, (request) => new StringContent(errorMessage));
        HttpClient httpClient = mockHttp.ToHttpClient();
        var tgtgClient = new TooGoodToGoApiClient(httpClient);
        var request = new AuthenticateByEmailRequest {
            Email = "my-email@microsoft.com"
        };
        
        // Act
        var exception = await Assert.ThrowsAsync<TooGoodToGoApiException>(() => tgtgClient.AuthenticateByEmail(request));

        // Assert
        exception.Message.Should().Be("Error while sending request to TooGoodToApi");
        exception.StatusCode.Should().Be(statusCode);
        exception.JsonResponse.Should().Be(errorMessage);
    }

    private string CreateAuthenticateByEmailJson(string pollingId) {
        return @$"{{
            ""polling_id"": ""{pollingId}""
        }}";
    }
    
    private string CreateAuthenticateByPollingIdJson(string accessToken, string refreshToken, int accessTokenTtlSeconds, string userId) {
        return @$"{{
            ""access_token"": ""{accessToken}"",
            ""refresh_token"": ""{refreshToken}"",
            ""access_token_ttl_seconds"": {accessTokenTtlSeconds},
            ""startup_data"": {{
                ""user"": {{
                    ""user_id"": ""{userId}""
                }}
            }}
        }}";
    }
    
    private string CreateRefreshTokenJson(string accessToken, string refreshToken, int accessTokenTtlSeconds) {
        return @$"{{
            ""access_token"": ""{accessToken}"",
            ""refresh_token"": ""{refreshToken}"",
            ""access_token_ttl_seconds"": {accessTokenTtlSeconds}
        }}";
    }

    private string CreateFavoritesItemsResponse(string displayName, int itemsAvailable) {
        return @$"{{
            ""items"": [
                {{
                    ""display_name"": ""{displayName}"",
                    ""items_available"": {itemsAvailable},
                }}                
            ]
        }}";
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