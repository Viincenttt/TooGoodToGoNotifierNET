using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;

public interface ITooGoodToGoApiClient {
    Task<AuthenticateByEmailResponse> AuthenticateByEmail(AuthenticateByEmailRequest request);
    Task<AuthenticateByPollingIdResponse?> AuthenticateByPollingId(AuthenticateByPollingIdRequest request);
    Task<RefreshAccessTokenResponse> RefreshAccessToken(RefreshAccessTokenRequest request);
    Task<FavoritesItemsResponse> GetFavoritesItems(string bearerToken, FavoritesItemsRequest request);
}