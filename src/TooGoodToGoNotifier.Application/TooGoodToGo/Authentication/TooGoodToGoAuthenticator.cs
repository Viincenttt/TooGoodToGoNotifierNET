using Microsoft.Extensions.Logging;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;
using TooGoodToGoNotifier.Domain.Exceptions;

namespace TooGoodToGoNotifier.Application.TooGoodToGo.Authentication; 

public class TooGoodToGoAuthenticator : ITooGoodToGoAuthenticator {
    private readonly ITooGoodToGoApiClient _tooGoodToGoApiClient;
    private readonly IAuthenticationCache _authenticationCache;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<TooGoodToGoAuthenticator> _logger;

    public TooGoodToGoAuthenticator(
        ITooGoodToGoApiClient tooGoodToGoApiClient, 
        IAuthenticationCache authenticationCache, 
        IDateTimeProvider dateTimeProvider, 
        ILogger<TooGoodToGoAuthenticator> logger) {
        
        _tooGoodToGoApiClient = tooGoodToGoApiClient;
        _authenticationCache = authenticationCache;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<AuthenticationDto> Authenticate(string email, CancellationToken cancellationToken = default) {
        AuthenticationDto? authenticationDto = await _authenticationCache.Get();
        if (authenticationDto != null) {
            authenticationDto = await RefreshAccessToken(authenticationDto);
            await _authenticationCache.Persist(authenticationDto);
            
            return authenticationDto;
        }
        else {
            authenticationDto = await RetrieveNewAuthenticationResult(cancellationToken, email);
            await _authenticationCache.Persist(authenticationDto);
            return authenticationDto;
        }
    }

    private async Task<AuthenticationDto> RefreshAccessToken(AuthenticationDto authenticationDto) {
        try {
            authenticationDto = await RefreshAccessTokenInternal(authenticationDto);
        }
        catch (TooGoodToGoApiException e) {
            _logger.LogError(e, "Error while refreshing token. Clearing authentication cache");
            await _authenticationCache.Clear();
        }

        return authenticationDto;
    }
    
    private async Task<AuthenticationDto> RefreshAccessTokenInternal(AuthenticationDto authenticationDto) {
        var request = new RefreshAccessTokenRequest {
            RefreshToken = authenticationDto.RefreshToken
        };
        RefreshAccessTokenResponse response = await _tooGoodToGoApiClient.RefreshAccessToken(request);
        return new AuthenticationDto {
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken,
            AccessTokenTtlSeconds = response.AccessTokenTtlSeconds,
            UserId = authenticationDto.UserId,
            CreatedOnUtc = _dateTimeProvider.UtcNow()
        };
    }

    private async Task<AuthenticationDto> RetrieveNewAuthenticationResult(CancellationToken cancellationToken, string email) {
        string pollingId = await AuthenticateByEmail(email);
        return await PollForAuthenticationResult(cancellationToken, email, pollingId);
    }

    private async Task<string> AuthenticateByEmail(string email) {
        var request = new AuthenticateByEmailRequest {
            Email = email
        };
        AuthenticateByEmailResponse response = await _tooGoodToGoApiClient.AuthenticateByEmail(request);
        return response.PollingId;
    }

    private async Task<AuthenticationDto> PollForAuthenticationResult(CancellationToken cancellationToken, string email, string pollingId) {
        const int maxRetries = 3;
        int retryCount = 0;
        while (retryCount < maxRetries) {
            cancellationToken.ThrowIfCancellationRequested();
            
            var request = new AuthenticateByPollingIdRequest {
                Email = email,
                RequestPollingId = pollingId
            };
            AuthenticateByPollingIdResponse? response = await _tooGoodToGoApiClient.AuthenticateByPollingId(request);
            if (response != null) {
                return new AuthenticationDto {
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    UserId = response.StartupData.User.UserId,
                    AccessTokenTtlSeconds = response.AccessTokenTtlSeconds,
                    CreatedOnUtc = _dateTimeProvider.UtcNow()
                };
            }

            retryCount++;
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }

        throw new TimeoutException("Polling e-mail was not opened in time");
    }
}