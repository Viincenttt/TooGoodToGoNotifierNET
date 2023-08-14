using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

namespace TooGoodToGoNotifier.Application.Tests.TooGoodToGo.Authentication; 

public class TooGoodToGoAuthenticatorTests {
    [Fact]
    public async Task When_the_cache_has_authentication_value_return_cached_value() {
        // Arrange
        var tgtgApi = new Mock<ITooGoodToGoApiClient>();
        var cachedAuthentication = CreateAuthenticationDto(14.August(2023));
        var authenticationCache = new Mock<IAuthenticationCache>();
        authenticationCache.Setup(x => x.Get()).ReturnsAsync(cachedAuthentication);
        var sut = Sut(tgtgApi, authenticationCache);

        // Act
        AuthenticationDto result = await sut.Authenticate("my-email", CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cachedAuthentication);
        tgtgApi.Verify(x => 
            x.AuthenticateByEmail(It.IsAny<AuthenticateByEmailRequest>()), Times.Never);
    }
    
    [Fact]
    public async Task When_the_cached_access_token_is_expired_refresh_token_is_called() {
        // Arrange
        var tgtgApi = new Mock<ITooGoodToGoApiClient>();
        var refreshedAuthentication = new RefreshAccessTokenResponse {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            AccessTokenTtlSeconds = 1000
        };
        tgtgApi.Setup(x => x.RefreshAccessToken(It.IsAny<RefreshAccessTokenRequest>()))
            .ReturnsAsync(refreshedAuthentication);
        var cachedAuthentication = CreateAuthenticationDto(14.August(2023));
        var dateTimeProvider = CreateDateTimeProvider(15.August(2023));
        var authenticationCache = new Mock<IAuthenticationCache>();
        authenticationCache.Setup(x => x.Get()).ReturnsAsync(cachedAuthentication);
        var sut = Sut(tgtgApi, authenticationCache, dateTimeProvider);

        // Act
        AuthenticationDto result = await sut.Authenticate("my-email", CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(refreshedAuthentication);
        authenticationCache.Verify(x => x.Persist(result));
        tgtgApi.Verify(x => 
            x.AuthenticateByEmail(It.IsAny<AuthenticateByEmailRequest>()), Times.Never);
        tgtgApi.Verify(x => 
            x.RefreshAccessToken(It.Is<RefreshAccessTokenRequest>(req => 
                req.RefreshToken == cachedAuthentication.RefreshToken)), Times.Once);
    }

    [Fact]
    public async Task When_no_cached_value_is_present_retrieve_new_credentials() {
        // Arrange
        string email = "my-email";
        var tgtgApi = new Mock<ITooGoodToGoApiClient>();
        var authenticateByEmailResponse = new AuthenticateByEmailResponse {
            PollingId = "polling-id"
        };
        tgtgApi.Setup(x => x.AuthenticateByEmail(It.IsAny<AuthenticateByEmailRequest>()))
            .ReturnsAsync(authenticateByEmailResponse);
        var authenticateByPollingResponse = new AuthenticateByPollingIdResponse {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            AccessTokenTtlSeconds = 1000,
            StartupData = new AuthenticateByPollingIdResponse.StartupDataResponse {
                User = new AuthenticateByPollingIdResponse.UserResponse {
                    UserId = "user-id"
                }
            }
        };
        tgtgApi.Setup(x => x.AuthenticateByPollingId(It.IsAny<AuthenticateByPollingIdRequest>()))
            .ReturnsAsync(authenticateByPollingResponse);
        var authenticationCache = new Mock<IAuthenticationCache>();
        var sut = Sut(tgtgApi, authenticationCache);
        
        // Act
        AuthenticationDto result = await sut.Authenticate(email, CancellationToken.None);
        
        // Assert
        result.UserId.Should().Be(authenticateByPollingResponse.StartupData.User.UserId);
        result.AccessToken.Should().Be(authenticateByPollingResponse.AccessToken);
        result.RefreshToken.Should().Be(authenticateByPollingResponse.RefreshToken);
        result.AccessTokenTtlSeconds.Should().Be(authenticateByPollingResponse.AccessTokenTtlSeconds);
        tgtgApi.Verify(x => 
            x.AuthenticateByEmail(It.Is<AuthenticateByEmailRequest>(request => request.Email == email)),
            Times.Once);
        tgtgApi.Verify(x => 
            x.AuthenticateByPollingId(It.Is<AuthenticateByPollingIdRequest>(request => 
                request.Email == email && request.RequestPollingId == authenticateByEmailResponse.PollingId)), 
            Times.Once);
    }

    private AuthenticationDto CreateAuthenticationDto(DateTime createdOn) {
        return new AuthenticationDto {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            UserId = "user-id",
            CreatedOnUtc = createdOn,
            AccessTokenTtlSeconds = 1000
        };
    }

    private Mock<IDateTimeProvider> CreateDateTimeProvider(DateTime now) {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(x => x.UtcNow()).Returns(now);
        return dateTimeProvider;
    }

    private static TooGoodToGoAuthenticator Sut(
        Mock<ITooGoodToGoApiClient>? tgtgApiClient = null,
        Mock<IAuthenticationCache>? authenticationCache = null,
        Mock<IDateTimeProvider>? dateTimeProvider = null,
        Mock<ILogger<TooGoodToGoAuthenticator>>? logger = null) {

        tgtgApiClient ??= new Mock<ITooGoodToGoApiClient>();
        authenticationCache ??= new Mock<IAuthenticationCache>();
        dateTimeProvider ??= new Mock<IDateTimeProvider>();
        logger ??= new Mock<ILogger<TooGoodToGoAuthenticator>>();

        return new TooGoodToGoAuthenticator(
            tgtgApiClient.Object, 
            authenticationCache.Object, 
            dateTimeProvider.Object,
            logger.Object);
    }
}