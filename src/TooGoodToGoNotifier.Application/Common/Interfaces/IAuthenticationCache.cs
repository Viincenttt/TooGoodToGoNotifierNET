using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;

namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface IAuthenticationCache {
    Task<AuthenticationDto?> Get();
    Task Persist(AuthenticationDto authenticationToCache);
    Task Clear();
}