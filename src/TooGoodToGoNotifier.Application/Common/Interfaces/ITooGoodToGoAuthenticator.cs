using TooGoodToGoNotifier.Application.TooGoodToGo.Authentication;

namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface ITooGoodToGoAuthenticator {
    Task<AuthenticationDto> Authenticate(string email, CancellationToken cancellationToken = default);
}