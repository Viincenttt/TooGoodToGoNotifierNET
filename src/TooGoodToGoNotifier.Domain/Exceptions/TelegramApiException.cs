using System.Net;

namespace TooGoodToGoNotifier.Domain.Exceptions; 

public class TelegramApiException : Exception {
    public HttpStatusCode? StatusCode { get; init; }
    public string? JsonResponse { get; init; }
    
    public TelegramApiException(HttpStatusCode statusCode, string jsonResponse) 
        : base ("Error while sending request to TelegramApi") {

        StatusCode = statusCode;
        JsonResponse = jsonResponse;
    }
}