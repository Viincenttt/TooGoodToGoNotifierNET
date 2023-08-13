namespace TooGoodToGoNotifier.Domain.Secrets; 

public record SecretDto {
    public required string Name { get; init; }
    
    public required string Value { get; init; }
    
    public required DateTime? CreatedOn { get; init; }
    
    public required DateTime? ExpiresOn { get; init; }
    
}