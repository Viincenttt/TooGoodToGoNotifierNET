namespace TooGoodToGoNotifier.Domain.Configuration; 

public record TooGoodToGoConfiguration {
    public required string Email { get; init; }
}