using TooGoodToGoNotifier.Application.Common.Interfaces;

namespace TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache; 

public class SecretsManagerAuthenticationCache : IAuthenticationCache {
    private const string UserIdSecretName = "TooGoodToGo__UserId";
    private const string AccessTokenSecretName = "TooGoodToGo__AccessToken";
    private const string RefreshTokenSecretName = "TooGoodToGo__RefreshToken";
    
    private readonly ISecretsManager _secretsManager;

    public SecretsManagerAuthenticationCache(ISecretsManager secretsManager) {
        _secretsManager = secretsManager;
    }

    public async Task<AuthenticationDto?> Get() {
        var userIdSecret = await _secretsManager.GetSecret(UserIdSecretName);
        var accessTokenSecret = await _secretsManager.GetSecret(AccessTokenSecretName);
        var refreshTokenSecret = await _secretsManager.GetSecret(RefreshTokenSecretName);
        if (userIdSecret != null && accessTokenSecret != null && refreshTokenSecret != null) {
            var accessTokenTtlSeconds = (accessTokenSecret.ExpiresOn - accessTokenSecret.CreatedOn)!.Value.TotalSeconds;
            var authenticationDto = new AuthenticationDto {
                UserId = userIdSecret.Value,
                AccessToken = accessTokenSecret.Value,
                RefreshToken = refreshTokenSecret.Value,
                AccessTokenTtlSeconds = (int)accessTokenTtlSeconds
            };

            return authenticationDto;
        }

        return null;
    }

    public async Task Persist(AuthenticationDto authenticationToCache) {
        await _secretsManager.SetSecret(UserIdSecretName, authenticationToCache.UserId);
        await _secretsManager.SetSecret(AccessTokenSecretName, authenticationToCache.AccessToken, authenticationToCache.ValidUntilUtc);
        await _secretsManager.SetSecret(RefreshTokenSecretName, authenticationToCache.RefreshToken);
    }

    public async Task Clear() {
        await _secretsManager.RemoveSecret(UserIdSecretName);
        await _secretsManager.RemoveSecret(AccessTokenSecretName);
        await _secretsManager.RemoveSecret(RefreshTokenSecretName);
    }
}