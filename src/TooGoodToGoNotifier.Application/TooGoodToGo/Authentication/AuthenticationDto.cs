namespace TooGoodToGoNotifier.Application.TooGoodToGo.Authentication; 

public class AuthenticationDto {
    public required string UserId { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required int AccessTokenTtlSeconds { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    
    public DateTime ValidUntilUtc => CreatedOnUtc + TimeSpan.FromSeconds(AccessTokenTtlSeconds);
}