namespace TooGoodToGoNotifier.Application.Common.Interfaces; 

public interface ICloudKeyValueCacheProvider {
    Task Save(string key, string value, TimeSpan? expiry = null);
    Task<string?> Get(string key);
}