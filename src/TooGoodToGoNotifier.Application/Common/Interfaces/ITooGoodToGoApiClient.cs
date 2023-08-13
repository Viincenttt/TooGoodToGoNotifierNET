using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Request;
using TooGoodToGoNotifier.Domain.ApiModels.TooGoodToGo.Response;

namespace TooGoodToGoNotifier.Application.Common.Interfaces;

public interface ITooGoodToGoApiClient {
    Task<AuthenticateByEmailResponse> AuthenticateByEmail(AuthenticateByEmailRequest request);
    Task<AuthenticateByPollingIdResponse?> AuthenticateByPollingId(AuthenticateByPollingIdRequest request);
    Task<RefreshAccessTokenResponse> RefreshAccessToken(RefreshAccessTokenRequest request);
    Task<FavoritesItemsResponse> GetFavoritesItems(string bearerToken, FavoritesItemsRequest request);
}