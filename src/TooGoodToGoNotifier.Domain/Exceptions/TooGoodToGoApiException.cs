﻿using System.Net;

namespace TooGoodToGoNotifier.Domain.Exceptions; 

public class TooGoodToGoApiException : Exception {
    public HttpStatusCode? StatusCode { get; init; }
    public string? JsonResponse { get; init; }
    
    public TooGoodToGoApiException(HttpStatusCode statusCode, string jsonResponse) 
        : base ($"Error while sending request to TooGoodToApi StatusCode={statusCode} Json={jsonResponse}") {

        StatusCode = statusCode;
        JsonResponse = jsonResponse;
    }
}