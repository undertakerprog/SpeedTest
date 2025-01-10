namespace Web.Src.Service
{
    public interface IRedisCacheService
    {
        Task<string?> GetCachedValueAsync(string key);
        Task SetCachedValueAsync(string key, string value, TimeSpan? expiry = null);

        Task<Dictionary<string, string>> GetRedisInfoAsync(string section);
    }
}