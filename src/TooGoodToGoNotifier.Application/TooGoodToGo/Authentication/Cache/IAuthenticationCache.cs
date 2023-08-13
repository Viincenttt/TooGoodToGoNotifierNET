namespace TooGoodToGoNotifier.Application.TooGoodToGo.Authentication.Cache; 

public interface IAuthenticationCache {
    Task<AuthenticationDto?> Get();
    Task Persist(AuthenticationDto authenticationToCache);
    Task Clear();
}