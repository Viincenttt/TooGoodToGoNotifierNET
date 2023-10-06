using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using TooGoodToGoNotifier.Application.Common.Interfaces;
using TooGoodToGoNotifier.Infrastructure.Azure.Configuration;

namespace TooGoodToGoNotifier.Infrastructure.Azure; 

public class BlobStorageCacheProvider : ICloudKeyValueCacheProvider {
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageCacheConfiguration _configuration;

    public BlobStorageCacheProvider(BlobServiceClient blobServiceClient, IOptions<BlobStorageCacheConfiguration> configuration) {
        _blobServiceClient = blobServiceClient;
        _configuration = configuration.Value;
    }

    public async Task Save(string key, string value, TimeSpan? expiry = null) {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.Container);
        BlobClient blobClient = containerClient.GetBlobClient(_configuration.Blob);
        var content = Encoding.UTF8.GetBytes(value);
        using var ms = new MemoryStream(content);
        await blobClient.UploadAsync(ms, overwrite: true);
    }

    public async Task<string?> Get(string key) {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_configuration.Container);
        BlobClient blobClient = containerClient.GetBlobClient(_configuration.Blob);
        if (await blobClient.ExistsAsync()) {
            var response = await blobClient.DownloadContentAsync();
            BinaryData data = response.Value.Content;
            return Encoding.UTF8.GetString(data);
        }

        return null;
    }
}