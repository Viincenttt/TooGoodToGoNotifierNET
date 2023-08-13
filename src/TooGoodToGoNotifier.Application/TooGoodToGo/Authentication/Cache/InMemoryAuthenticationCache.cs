namespace TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache; 

public class InMemoryAuthenticationCache : IAuthenticationCache {
    private AuthenticationDto? _cachedAuthentication = null;

    public Task<AuthenticationDto?> Get() {
        return Task.FromResult(_cachedAuthentication);
    }

    public Task Persist(AuthenticationDto authenticationToCache) {
        _cachedAuthentication = authenticationToCache;
        return Task.CompletedTask;
    }

    public Task Clear() {
        _cachedAuthentication = null;
        return Task.CompletedTask;
    }
}