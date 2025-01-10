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

        public async Task<Dictionary<string, string>> GetRedisInfoAsync(string section)
        {
            var info = await _database.ExecuteAsync("INFO", section);
            return info.ToString()
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(':', 2))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
        }

    }
}