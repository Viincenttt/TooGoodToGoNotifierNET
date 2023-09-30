namespace TooGoodToGoNotifier.Infrastructure.Azure.Configuration; 

public record BlobStorageCacheConfiguration {
    public required string Uri { get; init; }
    public required string Container { get; init; }
    public required string Blob { get; init; }
}