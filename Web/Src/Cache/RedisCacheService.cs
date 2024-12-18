using StackExchange.Redis;

namespace Web.Src.Service
{
    public class RedisCacheService(IConnectionMultiplexer redis) : IRedisCacheService
    {
        private readonly IDatabase _database = redis.GetDatabase();

        public async Task<string?> GetCachedValueAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task SetCachedValueAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, value, expiry);
        }
    }
}