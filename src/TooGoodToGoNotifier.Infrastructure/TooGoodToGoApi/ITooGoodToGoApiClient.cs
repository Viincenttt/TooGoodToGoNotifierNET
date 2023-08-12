using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Request;
using TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi.Models.Response;

namespace TooGoodToGoNotifier.Infrastructure.TooGoodToGoApi;

public interface ITooGoodToGoApiClient {
    Task<AuthenticateByEmailResponse> AuthenticateByEmail(AuthenticateByEmailRequest request);
    Task<AuthenticateByPollingIdResponse> AuthenticateByPollingId(AuthenticateByPollingIdRequest request);
}