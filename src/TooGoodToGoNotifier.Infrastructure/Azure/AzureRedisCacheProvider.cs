using StackExchange.Redis;
using TooGoodToGoNotifier.Application.Common.Interfaces;

namespace TooGoodToGoNotifier.Infrastructure.Azure; 

public class AzureRedisCacheProvider : ICloudKeyValueCacheProvider {
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public AzureRedisCacheProvider(IConnectionMultiplexer connectionMultiplexer) {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task Save(string key, string value, TimeSpan? expiry = null) {
        IDatabase database = _connectionMultiplexer.GetDatabase();
        await database.StringSetAsync(new RedisKey(key), new RedisValue(value), expiry);
    }
    
    public async Task<string?> Get(string key) {
        IDatabase database = _connectionMultiplexer.GetDatabase();
        return await database.StringGetAsync(new RedisKey(key));
    }
}